using ZXing.Net.Maui;

namespace ProyectoMotos;

public partial class CameraPage : ContentPage
{
    private bool isProcessing = false; // Indicador para evitar m�ltiples popups

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
        if (isProcessing) return; // Si ya est� procesando, no hacer nada

        isProcessing = true; // Marcar que est� procesando

        foreach (var barcode in e.Results)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("C�digo QR Detectado", barcode.Value, "OK");

                CameraReader.IsDetecting = false; // Detener la detecci�n
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
