using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Data;
using System.Diagnostics;

namespace ProyectoMotos
{
    public partial class MainPage : ContentPage
    {
        private string connectionString = "Server=motocut.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=motosv3;User ID=admin;     Password=motoCut$2024DB";

        public MainPage()
        {
            InitializeComponent();
        }


        // Método para obtener los datos del usuario autenticado
        private async Task<User?> ObtenerDatosUsuarioAsync(string email, string contrasena)
        {
            try
            {
                using (var conexion = new MySqlConnection(connectionString))
                {
                    await conexion.OpenAsync();

                    string query = "SELECT FirstName, LastName, Email FROM users WHERE Email = @user AND Password = @password";
                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@user", email);
                        cmd.Parameters.AddWithValue("@password", contrasena);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new User
                                {
                                    FirstName = reader.GetString(0),
                                    LastName = reader.GetString(1),
                                    Email = reader.GetString(2)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al conectar con la base de datos: " + ex.Message, "OK");
            }
            return null;
        }

        // Clase auxiliar para almacenar los datos del usuario
        public class User
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }


        // Método para registrarse
        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string email = emailEntry.Text;
            string password = passwordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Por favor ingrese un correo electrónico y una contraseña", "OK");
                return;
            }

            bool registroExitoso = await RegistrarUsuarioAsync(email, password);

            if (registroExitoso)
            {
                await DisplayAlert("Éxito", "Registro exitoso. Ahora puedes iniciar sesión.", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Error al registrar usuario. Intente nuevamente.", "OK");
            }
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = emailEntry.Text;
            string password = passwordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Por favor ingrese un correo electrónico y una contraseña", "OK");
                return;
            }

            var (loginExitoso, esAdmin) = await ValidarUsuarioAsync(email, password);

            // Depuración: Verificar valores de loginExitoso y esAdmin
            Debug.WriteLine($"Login exitoso: {loginExitoso}, esAdmin: {esAdmin}");

            if (loginExitoso)
            {
                await DisplayAlert("Éxito", "Login exitoso", "OK");

                if (esAdmin)
                {
                    // Depuración: Verificar que la condición esAdmin está funcionando
                    Debug.WriteLine("Usuario es Admin. Navegando a AdminPage");
                    await Navigation.PushAsync(new AdminPage(email, "", ""));
                }
                else
                {
                    // Depuración: Verificar que la condición esAdmin está funcionando
                    Debug.WriteLine("Usuario NO es Admin. Navegando a HomePage");
                    await Navigation.PushAsync(new HomePage(email, "", ""));  //Aqui 
                }
            }
            else
            {
                await DisplayAlert("Error", "Correo o contraseña incorrectos", "OK");
            }
        }

        private async Task<(bool, bool)> ValidarUsuarioAsync(string email, string contrasena)
        {
            try
            {
                using (var conexion = new MySqlConnection(connectionString))
                {
                    await conexion.OpenAsync();

                    string query = "SELECT Password, Admin FROM users WHERE Email = @user";
                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@user", email);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var dbPassword = reader["Password"].ToString();
                                var adminValue = reader.GetInt32("Admin"); // Usar GetInt32 para leer como entero directamente

                                bool isAdmin = adminValue == 1;
                                bool isValidPassword = dbPassword == contrasena;

                                // Mensajes de depuración para ver los valores obtenidos
                                Debug.WriteLine($"Contraseña de la base de datos: {dbPassword}");
                                Debug.WriteLine($"Valor de Admin desde la base de datos: {adminValue}");
                                Debug.WriteLine($"Password correcto: {isValidPassword}, esAdmin: {isAdmin}");

                                return (isValidPassword, isAdmin);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al conectar con la base de datos: " + ex.Message, "OK");
            }

            return (false, false);
        }



        // Método para registrar un nuevo usuario
        private async Task<bool> RegistrarUsuarioAsync(string email, string contrasena)
        {
            try
            {
                using (var conexion = new MySqlConnection(connectionString))
                {
                    await conexion.OpenAsync();

                    string query = "INSERT INTO users (Admin, FirstName, LastName, Email, Password) VALUES (0, '', '', @user, @password)";
                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@user", email);
                        cmd.Parameters.AddWithValue("@password", contrasena);

                        int filasAfectadas = await cmd.ExecuteNonQueryAsync();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al registrar usuario: " + ex.Message, "OK");
                return false;
            }
        }
    }
}
