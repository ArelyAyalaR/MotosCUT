using Microsoft.Maui.Controls;

namespace ProyectoMotos
{
    public partial class QrPage : ContentPage
    {
        public QrPage(string qrData)
        {
            InitializeComponent();
            BarcodeView.Value = qrData; // Asigna el valor para el c�digo QR
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Cierra la p�gina de QR y vuelve a la anterior
        }
    }
}
