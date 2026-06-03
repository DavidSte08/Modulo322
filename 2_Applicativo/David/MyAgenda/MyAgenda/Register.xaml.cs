using MyAgenda.Services;

namespace MyAgenda;

public partial class Register : ContentPage
{
    public Register()
    {
        InitializeComponent();
    }

    private async void OnCreateAccountClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";
        string confirm = ConfirmPasswordEntry.Text ?? "";

        // Validazione username
        if (string.IsNullOrEmpty(username))
        {
            await DisplayAlert("Errore", "Inserisci un nome utente.", "OK");
            return;
        }

        if (username.Length < 3)
        {
            await DisplayAlert("Errore", "Il nome utente deve avere almeno 3 caratteri.", "OK");
            return;
        }

        // Validazione password (centralizzata in PasswordValidator)
        string? errore = PasswordValidator.Validate(password);
        if (errore != null)
        {
            await DisplayAlert("Password non valida", errore, "OK");
            return;
        }

        if (password != confirm)
        {
            await DisplayAlert("Errore", "Le password non corrispondono.", "OK");
            return;
        }

        RegisterButton.IsEnabled = false;

        try
        {
            bool success = await AuthService.RegisterAsync(username, password);

            if (success)
            {
                await DisplayAlert("Successo", "Account creato! Ora puoi accedere.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Errore", "Username già in uso. Scegline un altro.", "OK");
            }
        }
        finally
        {
            RegisterButton.IsEnabled = true;
        }
    }
}
