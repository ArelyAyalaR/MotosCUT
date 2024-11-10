using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ProyectoMotos
{
    public partial class MainPage : ContentPage
    {
        private string connectionString = "Server=motocut.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=motosv3;User ID=admin;     Password=motoCut$2024DB";

        public MainPage()
        {
            InitializeComponent();
        }

        public static string EncriptarContrasena(string contrasena)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(contrasena));

                StringBuilder builder = new StringBuilder();
                foreach (byte t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }

                return builder.ToString();
            }
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
            // Redirige a la nueva página de registro
            await Navigation.PushAsync(new RegistroPage());
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

         
                    string contrasenaEncriptada = EncriptarContrasena(contrasena);

                    string query = "SELECT Password, Admin FROM users WHERE Email = @user";
                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@user", email);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var dbPassword = reader["Password"].ToString();
                                var adminValue = reader.GetInt32("Admin");

                                bool isAdmin = adminValue == 1;
                                bool isValidPassword = dbPassword == contrasenaEncriptada;

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
