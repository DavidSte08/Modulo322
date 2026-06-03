using MyAgenda.Services;

namespace MyAgenda;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Applica le preferenze visive salvate (colori, font)
        ThemeService.ApplicaTutto();

        MainPage = new NavigationPage(new Login());
    }
}
