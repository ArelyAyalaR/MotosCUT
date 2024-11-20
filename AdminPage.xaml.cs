using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Data;

namespace ProyectoMotos
{
    public partial class AdminPage : ContentPage
    {
        private string _emailUsuario;
        private string _firstName;
        private string _lastName;

        public AdminPage(string emailUsuario, string firstName, string lastName)
        {
            InitializeComponent();
            _emailUsuario = emailUsuario;
            _firstName = firstName;
            _lastName = lastName;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Verificar si los valores no están vacíos
            if (!string.IsNullOrEmpty(_firstName) && !string.IsNullOrEmpty(_lastName))
            {
                userFirstNameLabel.Text = _firstName;
                userLastNameLabel.Text = _lastName;
            }
            else
            {
                userFirstNameLabel.Text = "Nombre no disponible";
                userLastNameLabel.Text = "Apellido no disponible";
            }

            if (!string.IsNullOrEmpty(_emailUsuario))
            {
                userEmailLabel.Text = _emailUsuario;
            }
            else
            {
                userEmailLabel.Text = "Correo no disponible";
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private async void OnCamera(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CameraPage());
        }
    }
}
