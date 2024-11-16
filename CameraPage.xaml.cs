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
       // Dispatcher.Dispatch(() => {

         //   BarcodeResult.Text = $"{e.Results[0].Value} {e.Results[0].Format}"; 
        //});
    }
}