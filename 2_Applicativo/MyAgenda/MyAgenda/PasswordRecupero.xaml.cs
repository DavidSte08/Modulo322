namespace MyAgenda;

public partial class PasswordRecupero : ContentPage
{
    public PasswordRecupero()
    {
        InitializeComponent();
    }

    private async void OnSendClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim() ?? "";

        if (string.IsNullOrEmpty(email) || !email.Contains('@'))
        {
            await DisplayAlert("Errore", "Inserisci un indirizzo email valido.", "OK");
            return;
        }

        // TODO: implementare recupero reale via backend
        await DisplayAlert("Inviato", "Se l'account esiste, riceverai le istruzioni via email.", "OK");
        await Navigation.PopAsync();
    }
}
