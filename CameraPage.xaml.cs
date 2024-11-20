using ZXing.Net.Maui;

namespace ProyectoMotos;

public partial class CameraPage : ContentPage
{
    private bool isProcessing = false; // Indicador para evitar múltiples popups

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
        if (isProcessing) return; // Si ya está procesando, no hacer nada

        isProcessing = true; // Marcar que está procesando

        foreach (var barcode in e.Results)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Código QR Detectado", barcode.Value, "OK");

                CameraReader.IsDetecting = false; // Detener la detección
                isProcessing = false; // Liberar el indicador
            });
        }
    }

    private void OnStartScanClicked(object sender, EventArgs e)
    {
        CameraReader.IsDetecting = true;
        isProcessing = false; // Reiniciar el indicador cuando se inicia el escaneo
    }
}
