namespace MyAgenda
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Apre direttamente il Login avvolto in una NavigationPage
            MainPage = new NavigationPage(new Login());
        }
    }
}
