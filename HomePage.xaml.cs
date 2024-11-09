// HomePage.xaml.cs

namespace ProyectoMotos
{
    public partial class HomePage : ContentPage
    {
        public HomePage(string firstName, string lastName, string email)
        {
            InitializeComponent();
            // Mostrar los datos del usuario en el Label de bienvenida
            welcomeLabel.Text = $"Bienvenido, {firstName} {lastName}!\nCorreo: {email}";
        }

        // Método para cerrar sesión y regresar a MainPage
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
