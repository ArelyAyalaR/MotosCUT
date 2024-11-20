using System.Diagnostics;
using ZXing.Net.Maui;

namespace ProyectoMotos
{
    public partial class CameraPage : ContentPage
    {
        private bool isProcessing = false; 
        private string qrCodeData; 

        public CameraPage()
        {
            InitializeComponent();
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

        private void CameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
        {
            if (isProcessing) return; 

            isProcessing = true; 

            foreach (var barcode in e.Results)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    qrCodeData = barcode.Value;

                    await DisplayAlert("Código QR Detectado", qrCodeData, "OK");

                    CameraReader.IsDetecting = false; 
                    isProcessing = false;
                    if (!string.IsNullOrEmpty(qrCodeData))
                    {
                        Debug.WriteLine($"El código QR es: {qrCodeData}");
                    }
                    else
                    {
                        Debug.WriteLine("No se ha detectado ningún código QR.");
                    }
                });
            }
        }

        private void OnStartScanClicked(object sender, EventArgs e)
        {
            CameraReader.IsDetecting = true;
            isProcessing = false;
        }

        private void UseQRCodeData()
        {
            if (!string.IsNullOrEmpty(qrCodeData))
            {

                Console.WriteLine($"El código QR es: {qrCodeData}");
            }
            else
            {
                Console.WriteLine("No se ha detectado ningún código QR.");
            }
        }
    }
}
