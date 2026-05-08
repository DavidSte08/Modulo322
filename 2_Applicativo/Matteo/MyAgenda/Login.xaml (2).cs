namespace MyAgenda;

public partial class Login : ContentPage
{
    public Login()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Errore", "Inserisci nome utente e password.", "OK");
            return;
        }

        bool success = await AuthService.LoginAsync(username, password);

        if (success)
        {
            AuthService.SaveCurrentUser(username); // salva l'utente loggato
            Application.Current!.MainPage = new AppShell();
        }
        else
        {
            await DisplayAlert("Errore", "Credenziali non valide.", "OK");
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Register());
    }

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PasswordRecupero());
    }
}
