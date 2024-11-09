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

            // Aquí puedes cargar los datos directamente
            userNameLabel.Text = $"{_firstName} {_lastName}";
            userEmailLabel.Text = _emailUsuario;
        }
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }

}
