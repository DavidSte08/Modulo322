using MyAgenda.Services;

namespace MyAgenda;

public partial class Account : ContentPage
{
    public Account()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        string username = SessionService.Username;
        LblNomeUtente.Text = username;
        LblNomeUtenteVal.Text = username;

        // Carica preferenze salvate
        LblStileColori.Text = ThemeService.StileColori;
        LblStileFont.Text = ThemeService.StileFont;
        LblSuonoNotifiche.Text = ThemeService.SuonoNotifiche;
    }

    private async void OnHomeClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("..");

    // ── 1. DETTAGLI UTENTE ─────────────────────────────────────────────────

    private void OnDettagliCardTapped(object sender, TappedEventArgs e)
    {
        if (BtnApplicaDettagli.IsVisible) return;

        TxtNomeUtente.Text = LblNomeUtente.Text;
        TxtPassword.Text = string.Empty;
        TxtPasswordOld.Text = string.Empty;
        LblForzaPassword.Text = string.Empty;

        BtnApplicaDettagli.IsVisible = true;
        ImgEditIcon.IsVisible = true;

        // Mostra campi di modifica
        LblNomeUtenteVal.IsVisible = false;
        BorderNomeUtente.IsVisible = true;

        LblPassword.IsVisible = false;
        BorderPasswordOld.IsVisible = true;
        NuovaPasswordLayout.IsVisible = true;

        // Aggiorna indicatore forza password mentre si digita
        TxtPassword.TextChanged += OnNuovaPasswordChanged;
    }

    private void OnNuovaPasswordChanged(object? sender, TextChangedEventArgs e)
    {
        string pwd = e.NewTextValue ?? "";
        if (string.IsNullOrEmpty(pwd))
        {
            LblForzaPassword.Text = string.Empty;
            return;
        }

        int punti = 0;
        if (pwd.Length >= 8) punti++;
        if (pwd.Any(char.IsUpper)) punti++;
        if (pwd.Any(char.IsLower)) punti++;
        if (pwd.Any(char.IsDigit)) punti++;
        if (pwd.Any(c => "!@#$%^&*()_+-=[]{}|;':\",./<>?".Contains(c))) punti++;

        (LblForzaPassword.Text, LblForzaPassword.TextColor) = punti switch
        {
            <= 2 => ("🔴 Debole", Colors.Red),
            3 or 4 => ("🟡 Media", Color.FromArgb("#D97706")),
            _ => ("🟢 Forte", Color.FromArgb("#16A34A"))
        };
    }

    private async void OnApplicaDettagliClicked(object sender, EventArgs e)
    {
        // Chiudi i campi
        BtnApplicaDettagli.IsVisible = false;
        ImgEditIcon.IsVisible = false;
        TxtPassword.TextChanged -= OnNuovaPasswordChanged;

        LblNomeUtenteVal.IsVisible = true;
        BorderNomeUtente.IsVisible = false;
        LblPassword.IsVisible = true;
        BorderPasswordOld.IsVisible = false;
        NuovaPasswordLayout.IsVisible = false;
        LblForzaPassword.Text = string.Empty;

        // ── Cambio username ──
        string nuovoNome = TxtNomeUtente.Text?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(nuovoNome) && nuovoNome != SessionService.Username)
        {
            string vecchioUsername = SessionService.Username;
            bool ok = AuthService.CambiaUsername(vecchioUsername, nuovoNome);
            if (ok)
            {
                StorageService.MigraFile(vecchioUsername, nuovoNome);
                SessionService.Start(nuovoNome);
                LblNomeUtente.Text = nuovoNome;
                LblNomeUtenteVal.Text = nuovoNome;
                await DisplayAlert("✅ Successo", "Username aggiornato.", "OK");
            }
            else
            {
                await DisplayAlert("Errore", "Username già in uso o non valido.", "OK");
            }
        }

        // ── Cambio password ──
        string vecchiaPass = TxtPasswordOld.Text ?? string.Empty;
        string nuovaPass = TxtPassword.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(vecchiaPass) && string.IsNullOrWhiteSpace(nuovaPass))
            return;

        if (string.IsNullOrWhiteSpace(vecchiaPass) || string.IsNullOrWhiteSpace(nuovaPass))
        {
            await DisplayAlert("Errore", "Compila entrambi i campi password.", "OK");
            return;
        }

        string? errore = PasswordValidator.Validate(nuovaPass);
        if (errore != null)
        {
            await DisplayAlert("Password non valida", errore, "OK");
            return;
        }

        bool okPwd = AuthService.CambiaPassword(SessionService.Username, vecchiaPass, nuovaPass);
        if (okPwd)
            await DisplayAlert("✅ Successo", "Password aggiornata.", "OK");
        else
            await DisplayAlert("Errore", "La vecchia password non è corretta.", "OK");

        TxtPasswordOld.Text = string.Empty;
        TxtPassword.Text = string.Empty;
    }

    // ── 2. PERSONALIZZAZIONE ───────────────────────────────────────────────

    private void OnPersonalizzazioneCardTapped(object sender, TappedEventArgs e)
    {
        if (BtnApplicaPersonalizzazione.IsVisible) return;

        // Pre-seleziona valori correnti
        PkrStileColori.SelectedItem = ThemeService.StileColori;
        PkrStileFont.SelectedItem = ThemeService.StileFont;
        PkrSuonoNotifiche.SelectedItem = ThemeService.SuonoNotifiche;

        BtnApplicaPersonalizzazione.IsVisible = true;
        PkrStileColori.IsVisible = true;
        PkrStileFont.IsVisible = true;
        PkrSuonoNotifiche.IsVisible = true;
        LblStileColori.IsVisible = false;
        LblStileFont.IsVisible = false;
        LblSuonoNotifiche.IsVisible = false;
    }

    private void OnApplicaPersonalizzazioneClicked(object sender, EventArgs e)
    {
        // Salva e applica immediatamente — visibile senza uscire dall'app
        if (PkrStileColori.SelectedItem is string colori)
        {
            ThemeService.SalvaColori(colori);   // ← applica subito i colori
            LblStileColori.Text = colori;
        }

        if (PkrStileFont.SelectedItem is string font)
        {
            ThemeService.SalvaFont(font);        // ← applica subito il font
            LblStileFont.Text = font;
        }

        if (PkrSuonoNotifiche.SelectedItem is string suono)
        {
            ThemeService.SalvaSuono(suono);
            LblSuonoNotifiche.Text = suono;
        }

        BtnApplicaPersonalizzazione.IsVisible = false;
        PkrStileColori.IsVisible = false;
        PkrStileFont.IsVisible = false;
        PkrSuonoNotifiche.IsVisible = false;
        LblStileColori.IsVisible = true;
        LblStileFont.IsVisible = true;
        LblSuonoNotifiche.IsVisible = true;
    }

    // ── LOGOUT ─────────────────────────────────────────────────────────────

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        SessionService.End();
        Application.Current!.MainPage = new NavigationPage(new Login());
    }
}
