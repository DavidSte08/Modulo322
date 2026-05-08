namespace MyAgenda
{
    public partial class HomePage : ContentPage
    {

        public HomePage()
        {
            InitializeComponent();

        }
        private async void OnAccountClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Account");
        }
        private async void OnAttivitaClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Attivita");
        }
        private async void OnMaterieClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Scuola");
        }



    }

}
