using ZXing.Net.Maui;

namespace ProyectoMotos;

public partial class CameraPage : ContentPage
{
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
        foreach (var barcode in e.Results)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Código QR Detectado", barcode.Value, "OK");

                CameraReader.IsDetecting = false;
            });
        }
    }

    private void OnStartScanClicked(object sender, EventArgs e)
    {
        CameraReader.IsDetecting = true;
    }
}
