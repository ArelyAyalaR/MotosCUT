using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace ProyectoMotos
{
    public partial class Login : ContentPage
    {
        private string connectionString = "Server=motocut.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=motosv3;User ID=admin;Password=motoCut$2024DB";

        public Login()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Cambia la variable "username" a "email" para reflejar que es el campo de Email
            string email = usernameEntry.Text;  // Si renombraste "usernameEntry" a "emailEntry" en XAML, cambia aquí también
            string password = passwordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Error", "Por favor ingrese un email y una contraseña", "OK");
                return;
            }

            bool loginExitoso = await ValidarUsuarioAsync(email, password);

            if (loginExitoso)
            {
                await DisplayAlert("Éxito", "Login exitoso", "OK");
                // Navegar a otra página
                await Navigation.PushAsync(new MainPage(email));
            }
            else
            {
                await DisplayAlert("Error", "Email o contraseña incorrectos", "OK");
            }
        }

        // Cambia los parámetros y la consulta para utilizar Email y Password en lugar de Usuario y Contrasena
        private async Task<bool> ValidarUsuarioAsync(string email, string password)
        {
            try
            {
                using (var conexion = new MySqlConnection(connectionString))
                {
                    await conexion.OpenAsync();

                    // Actualiza la consulta para verificar Email y Password
                    string query = "SELECT COUNT(*) FROM users WHERE Email = @Email AND Password = @Password";
                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

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
    }
}
