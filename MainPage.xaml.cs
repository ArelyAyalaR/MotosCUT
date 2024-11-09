using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace ProyectoMotos
{
    public partial class MainPage : ContentPage
    {
        private string connectionString = "Server=motocut.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=motosv3;User ID=admin;     Password=motoCut$2024DB";

        public MainPage()
        {
            InitializeComponent();
        }

        // Método para iniciar sesión
        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = emailEntry.Text;
            string password = passwordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Por favor ingrese un correo electrónico y una contraseña", "OK");
                return;
            }

            bool loginExitoso = await ValidarUsuarioAsync(email, password);

            if (loginExitoso)
            {
                await DisplayAlert("Éxito", "Login exitoso", "OK");
                // Redirige a otra página, por ejemplo:
                // await Navigation.PushAsync(new HomePage());
            }
            else
            {
                await DisplayAlert("Error", "Correo o contraseña incorrectos", "OK");
            }
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

        // Método para validar al usuario en la base de datos
        private async Task<bool> ValidarUsuarioAsync(string email, string contrasena)
        {
            try
            {
                using (var conexion = new MySqlConnection(connectionString))
                {
                    await conexion.OpenAsync();

                    string query = "SELECT COUNT(*) FROM users WHERE Email = @user AND Password = @password";
                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@user", email);
                        cmd.Parameters.AddWithValue("@password", contrasena);

                        var resultado = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(resultado) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al conectar con la base de datos: " + ex.Message, "OK");
                return false;
            }
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
