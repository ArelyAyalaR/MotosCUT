using ZXing.Net.Maui;

namespace ProyectoMotos;

public partial class CameraPage : ContentPage
{
    public CameraPage()
    {
        InitializeComponent();
    }

    private void OnBarcodeDetected(object sender, BarcodeDetectionEventArgs e)
    {
        var result = e.Results.FirstOrDefault()?.Value;

        if (!string.IsNullOrEmpty(result))
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Código Detectado", $"Contenido: {result}", "OK");
                BarcodeReader.IsScanning = false;
            });
        }
    }
    private void OnStopScanningClicked(object sender, EventArgs e)
    {
        BarcodeReader.IsScanning = false;
    }
}
