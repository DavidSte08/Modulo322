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
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Errore", "Compila tutti i campi.", "OK");
            return;
        }

        if (username.Length < 3)
        {
            await DisplayAlert("Errore", "Il nome utente deve avere almeno 3 caratteri.", "OK");
            return;
        }

        // Validazione password
        string? passwordError = ValidatePassword(password);
        if (passwordError != null)
        {
            await DisplayAlert("Password non valida", passwordError, "OK");
            return;
        }

        // Conferma password
        if (password != confirm)
        {
            await DisplayAlert("Errore", "Le password non corrispondono.", "OK");
            return;
        }

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

    private string? ValidatePassword(string password)
    {
        if (password.Length < 8)
            return "La password deve avere almeno 8 caratteri.";

        if (password.Length > 64)
            return "La password non può superare i 64 caratteri.";

        if (!password.Any(char.IsUpper))
            return "La password deve contenere almeno una lettera maiuscola.";

        if (!password.Any(char.IsLower))
            return "La password deve contenere almeno una lettera minuscola.";

        if (!password.Any(char.IsDigit))
            return "La password deve contenere almeno un numero.";

        if (!password.Any(c => "!@#$%^&*()_+-=[]{}|;':\",./<>?".Contains(c)))
            return "La password deve contenere almeno un carattere speciale (es. !, @, #, $).";

        return null; // password valida
    }
}
