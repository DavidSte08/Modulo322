namespace MyAgenda;

// Classe "PasswordRecupero" per corrispondere a x:Class="MyAgenda.PasswordRecupero" in PasswordRecupero.xaml
public partial class PasswordRecupero : ContentPage
{
    public PasswordRecupero()
    {
        InitializeComponent();
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlert("Errore", "Inserisci la tua email.", "OK");
            return;
        }

        await DisplayAlert("Inviato", "Controlla la tua email per le istruzioni.", "OK");
        await Navigation.PopAsync();
    }
}
