namespace MyAgenda
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registra tutte le tue pagine
            Routing.RegisterRoute("Account", typeof(Account));
            Routing.RegisterRoute("HomePage", typeof(HomePage));
            Routing.RegisterRoute("Attivita", typeof(Attivita));
            Routing.RegisterRoute("Scuola", typeof(Scuola));
            Routing.RegisterRoute("Login", typeof(Login));
        }
    }
}
