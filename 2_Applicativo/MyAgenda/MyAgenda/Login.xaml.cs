using MyAgenda.Services;

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

        // Disabilita il bottone durante il login per evitare doppi click
        LoginButton.IsEnabled = false;

        try
        {
            bool success = await AuthService.LoginAsync(username, password);

            if (success)
            {
                SessionService.Start(username);
                Application.Current!.MainPage = new AppShell();
            }
            else
            {
                await DisplayAlert("Errore", "Nome utente o password non corretti.", "OK");
            }
        }
        finally
        {
            LoginButton.IsEnabled = true;
        }
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
        => await Navigation.PushAsync(new Register());

    private async void OnForgotPasswordTapped(object sender, EventArgs e)
        => await Navigation.PushAsync(new PasswordRecupero());
}
