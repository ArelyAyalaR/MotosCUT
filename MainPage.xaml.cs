using Microsoft.Maui.Controls;

namespace ProyectoMotos
{
    public partial class MainPage : ContentPage
    {
        public MainPage(string usuarioEmail)
        {
            InitializeComponent();
            MostrarQRCode(usuarioEmail);
        }

        private void MostrarQRCode(string usuarioEmail)
        {
            BarcodeView.Value = usuarioEmail; 
            BarcodeView.IsVisible = true; // Cambia la visibilidad para mostrar el QR
        }
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync(); // Regresa a la pantalla de inicio de sesión
        }
    }
}
