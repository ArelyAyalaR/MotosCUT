using Microsoft.Maui.Controls;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace ProyectoMotos
{
    public partial class RegistroPage : ContentPage
    {
        private string connectionString = "Server=motocut.cfko0iqhcsi0.us-east-1.rds.amazonaws.com;Database=motosv3;User ID=admin;Password=motoCut$2024DB";

        public RegistroPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Recoger los datos de los controles de la UI
            string firstName = firstNameEntry.Text;
            string lastName = lastNameEntry.Text;
            string email = emailEntry.Text;
            string password = passwordEntry.Text;
            string licensePlate = licensePlateEntry.Text;
            string model = modelEntry.Text;
            string brand = brandEntry.Text;

            // Validar que todos los campos están completos
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(licensePlate) || string.IsNullOrEmpty(model) || string.IsNullOrEmpty(brand))
            {
                await DisplayAlert("Error", "Por favor complete todos los campos", "OK");
                return;
            }

            // Registrar el usuario y la moto en la base de datos
            bool registroExitoso = await RegistrarUsuarioYMotocicletaAsync(firstName, lastName, email, password, licensePlate, model, brand);

            if (registroExitoso)
            {
                await DisplayAlert("Éxito", "Registro exitoso", "OK");
                // Redirigir a la página de inicio o login
                await Navigation.PopAsync(); // Para regresar a la página anterior (si es necesario)
            }
            else
            {
                await DisplayAlert("Error", "Hubo un error al registrar el usuario y la moto. Intente nuevamente.", "OK");
            }
        }

        private async Task<bool> RegistrarUsuarioYMotocicletaAsync(string firstName, string lastName, string email, string password,
                                                                  string licensePlate, string model, string brand)
        {
            try
            {
                using (var conexion = new MySqlConnection(connectionString))
                {
                    await conexion.OpenAsync();

                    // 1. Insertar usuario y obtener UserID generado
                    string queryUser = "INSERT INTO users (FirstName, LastName, Email, Password) VALUES (@firstName, @lastName, @email, @password)";
                    using (var cmdUser = new MySqlCommand(queryUser, conexion))
                    {
                        cmdUser.Parameters.AddWithValue("@firstName", firstName);
                        cmdUser.Parameters.AddWithValue("@lastName", lastName);
                        cmdUser.Parameters.AddWithValue("@email", email);
                        cmdUser.Parameters.AddWithValue("@password", password);

                        int userRowsAffected = await cmdUser.ExecuteNonQueryAsync();

                        if (userRowsAffected == 0)
                            return false;

                        int userID = (int)cmdUser.LastInsertedId;

                        // 2. Registrar la moto con UserID asociado
                        string queryMoto = "INSERT INTO motorcycles (LicencePlate, Model, Brand) VALUES (@licensePlate, @model, @brand)";
                        using (var cmdMoto = new MySqlCommand(queryMoto, conexion))
                        {
                            cmdMoto.Parameters.AddWithValue("@licensePlate", licensePlate);
                            cmdMoto.Parameters.AddWithValue("@model", model);
                            cmdMoto.Parameters.AddWithValue("@brand", brand);

                            int motoRowsAffected = await cmdMoto.ExecuteNonQueryAsync();

                            if (motoRowsAffected == 0)
                                return false;
                        }

                        // 3. Asociar usuario y moto en tabla qrcode (si es necesario)
                        string queryQRCode = "INSERT INTO qrcode (UserID, FirstName, LastName, LicencePlate, Brand) " +
                                             "VALUES (@userID, @firstName, @lastName, @licensePlate, @brand)";
                        using (var cmdQRCode = new MySqlCommand(queryQRCode, conexion))
                        {
                            cmdQRCode.Parameters.AddWithValue("@userID", userID);
                            cmdQRCode.Parameters.AddWithValue("@firstName", firstName);
                            cmdQRCode.Parameters.AddWithValue("@lastName", lastName);
                            cmdQRCode.Parameters.AddWithValue("@licensePlate", licensePlate);
                            cmdQRCode.Parameters.AddWithValue("@brand", brand);

                            int qrRowsAffected = await cmdQRCode.ExecuteNonQueryAsync();

                            if (qrRowsAffected == 0)
                                return false;
                        }
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Error al registrar los datos: " + ex.Message, "OK");
                return false;
            }
        }

    }
}
