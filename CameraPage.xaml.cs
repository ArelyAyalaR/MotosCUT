using System.Text.RegularExpressions;
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
                        var qrIdMatch = Regex.Match(qrCodeData, @"QR ID:\s*(\d+)");

                        if (qrIdMatch.Success)
                        {
                            string qrId = qrIdMatch.Groups[1].Value;
                            Debug.WriteLine($"El QR ID es: {qrId}");

                            Console.WriteLine($"El QR ID es: {qrId}");

                        }
                        else
                        {
                            Debug.WriteLine("No se encontró un QR ID en el código QR.");
                        }
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

        private void GetDataByQRId(string qrId)
        {
            Console.WriteLine($"Obteniendo información para el QR ID: {qrId}");
        }
    }
}
