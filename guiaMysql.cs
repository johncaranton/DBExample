using System;
using System.Data;
using MySql.Data.MySqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace testDB
{
    public partial class Form1 : Form
    {
        // Cadena de conexión para la base de datos MySQL
        string connStr = "server=192.168.1.14;user=usertestdb;database=testdb;port=3306;password=ajedrez1";

        public Form1()
        {
            InitializeComponent();
        }

        // Botón para agregar 10 elementos a la lista
        private void button1_Click(object sender, EventArgs e)
        {
            int index = listView1.Items.Count; // Número de elementos actuales en la lista

            // Agrega 10 nuevos elementos a la lista
            for (int i = index; i < index + 10; i++)
            {
                listView1.Items.Add("a");
                listView1.Items[i].SubItems.Add(i.ToString());
                listView1.Items[i].SubItems.Add(i.ToString());
                listView1.Items[i].SubItems.Add("c5" + i.ToString());
            }

            // Modifica un valor específico en la lista
            listView1.Items[2].SubItems[2].Text = "NuevoValor";
        }

        // Botón para limpiar y reiniciar la lista
        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Clear();
            init_listview();
        }

        // Botón para limpiar la lista y ejecutar una consulta en la base de datos
        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Clear();
            init_listview();
            ExecuteQuery("SELECT * FROM usuarios");
        }

        // Inicializa las columnas del ListView
        void init_listview()
        {
            listView1.Columns.Add("id");
            listView1.Columns.Add("nombre");
            listView1.Columns.Add("apellido");
            listView1.Columns.Add("email");
            listView1.Columns.Add("telefono");
            listView1.Columns.Add("fecha_registro");
            listView1.View = View.Details; // Modo "detalles"
        }

        // Método para probar la conexión con la base de datos
        private void testconection()
        {
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open(); // Abre la conexión
                // Aquí se pueden realizar operaciones con la base de datos
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                conn.Close(); // Cierra la conexión
                Console.WriteLine("Done.");
            }
        }

        // Ejecuta una consulta y muestra los resultados en el ListView
        public void ExecuteQuery(string query)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connStr))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                int i = 0;
                                while (reader.Read())
                                {
                                    // Agrega los datos obtenidos al ListView
                                    listView1.Items.Add(reader.GetInt32(0).ToString());
                                    listView1.Items[i].SubItems.Add(reader.GetString(1));
                                    listView1.Items[i].SubItems.Add(reader.GetString(2));
                                    listView1.Items[i].SubItems.Add(reader.GetString(3));
                                    i++;
                                }
                            }
                            else
                            {
                                Console.WriteLine("No rows found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al ejecutar la consulta: {ex.Message}");
            }
        }

        // Actualiza un usuario en la base de datos
        public void ActualizarUsuario(int id, string nuevoNombre, string nuevoApellido)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connStr))
                {
                    connection.Open();

                    string query = "UPDATE usuarios SET nombre = @nombre, apellido = @apellido WHERE id = @id";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@nombre", nuevoNombre);
                        command.Parameters.AddWithValue("@apellido", nuevoApellido);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Usuario actualizado exitosamente.");
                        }
                        else
                        {
                            Console.WriteLine("No se encontró un usuario con el ID especificado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al actualizar el usuario: " + ex.Message);
            }
        }

        // Inserta un nuevo usuario en la base de datos
        public void InsertarUsuario(string nombre, string apellido, string email, string telefono, DateTime fechaRegistro)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connStr))
                {
                    connection.Open();

                    string query = "INSERT INTO usuarios (nombre, apellido, email, telefono, fecha_registro) VALUES (@nombre, @apellido, @correo, @telefono, @fecha)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nombre", nombre);
                        command.Parameters.AddWithValue("@apellido", apellido);
                        command.Parameters.AddWithValue("@correo", email);
                        command.Parameters.AddWithValue("@telefono", telefono);
                        command.Parameters.AddWithValue("@fecha", fechaRegistro);

                        command.ExecuteNonQuery();
                        Console.WriteLine("Nuevo usuario insertado exitosamente.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al insertar el usuario: " + ex.Message);
            }
        }

        // Botón para insertar un usuario con valores aleatorios
        private void update_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            DateTime dateTime = DateTime.Now;
            InsertarUsuario(RandomString(8), RandomString(8), RandomString(5), rnd.Next(35).ToString(), dateTime);
        }

        // Genera una cadena aleatoria
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Botón para actualizar un usuario con valores aleatorios
        private void button4_Click(object sender, EventArgs e)
        {
            ActualizarUsuario(2, RandomString(8), RandomString(8));
        }
    }
}
