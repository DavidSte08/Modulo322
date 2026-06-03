namespace MyAgenda;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(Account), typeof(Account));
        Routing.RegisterRoute(nameof(Attivita), typeof(Attivita));
        Routing.RegisterRoute(nameof(Scuola), typeof(Scuola));
    }
}
