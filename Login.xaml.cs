using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace ProyectoMotos;

public partial class NewPage1 : ContentPage
{
    // Actualiza las credenciales aqu� seg�n tu configuraci�n
    private string connectionString = "Server=motocut.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=motosv3;User ID=admin;     Password=motoCut$2024DB";

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = usernameEntry.Text;
        string password = passwordEntry.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Por favor ingrese un usuario y una contrase�a", "OK");
            return;
        }

        bool loginExitoso = await ValidarUsuarioAsync(username, password);

        if (loginExitoso)
        {
            await DisplayAlert("�xito", "Login exitoso", "OK");
            // Navegar a otra p�gina, por ejemplo:
            // await Navigation.PushAsync(new MainPage());
        }
        else
        {
            await DisplayAlert("Error", "Usuario o contrase�a incorrectos", "OK");
        }
    }

    private async Task<bool> ValidarUsuarioAsync(string email, string contrasena)
    {
        try
        {
            using (var conexion = new MySqlConnection(connectionString))
            {
                await conexion.OpenAsync();

                // Ajuste de la consulta con los nuevos nombres de columna
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
}
