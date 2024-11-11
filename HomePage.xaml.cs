using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace ProyectoMotos
{
    public partial class HomePage : ContentPage
    {
        private string _emailUsuario;
        private string _firstName;
        private string _lastName;
        private int _userID; // ID del usuario que inició sesión
        private string connectionString = "Server=motocut.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=motosv3;User ID=admin;Password=motoCut$2024DB";

        public HomePage(string emailUsuario, string firstName, string lastName, int userID)
        {
            InitializeComponent();
            _emailUsuario = emailUsuario;
            _firstName = firstName;
            _lastName = lastName;
            _userID = userID;

            // Muestra un mensaje de bienvenida personalizado
            welcomeLabel.Text = $"¡Bienvenido, {_firstName} {_lastName}!";
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            // Regresa a la raíz de la navegación, como la pantalla de inicio de sesión
            await Navigation.PopToRootAsync();
        }

        private async void OnInsertTextClicked(object sender, EventArgs e)
        {
            try
            {
                using (var conexion = new MySqlConnection(connectionString))
                {
                    await conexion.OpenAsync();

                    // Construye la consulta SQL para el usuario actual
                    string query = @"
                        SELECT 
                            qrcode.QrID,
                            users.UsersID,
                            users.FirstName,
                            users.LastName,
                            users.Email,
                            motorcycles.LicencePlate,
                            motorcycles.Model,
                            motorcycles.Brand
                        FROM 
                            qrcode
                        JOIN 
                            users ON qrcode.UserID = users.UsersID
                        JOIN 
                            motorcycles ON qrcode.LicencePlate = motorcycles.LicencePlate
                        WHERE 
                            qrcode.UserID = @userID";

                    using (var cmd = new MySqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@userID", _userID);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                // Concatenar los datos en una cadena de texto para el QR
                                string qrData = $"QR ID: {reader["QrID"]}\n" +
                                                   $"Usuario: {reader["FirstName"]} {reader["LastName"]}\n" +
                                                   $"Email: {reader["Email"]}\n" +
                                                   $"Moto: {reader["LicencePlate"]}, {reader["Model"]}, {reader["Brand"]}";

                                await Navigation.PushAsync(new QrPage(qrData));
                            }
                            else
                            {
                                await DisplayAlert("Error", "No se encontraron datos para este usuario.", "OK");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al realizar la consulta: " + ex.Message, "OK");
            }
        }
    }
}
