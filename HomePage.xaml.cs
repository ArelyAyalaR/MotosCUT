namespace ProyectoMotos
{

    public partial class HomePage : ContentPage
    {
        private string _emailUsuario;
        private string _firstName;
        private string _lastName;

        public HomePage(string emailUsuario, string firstName, string lastName)
        {
            InitializeComponent();
            _emailUsuario = emailUsuario;
            _firstName = firstName;
            _lastName = lastName;

            // Puedes manejar la configuración de la interfaz de usuario en otro método si es necesario
        }
        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }


}
