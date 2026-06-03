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

        // Controlla se è presente un nome utente valido salvato nella sessione dal login
        if (!string.IsNullOrEmpty(Sessione.Username))
        {
            // Aggiorna il testo dell'etichetta XAML con il vero nome utente
            LblNomeUtente.Text = Sessione.Username;
        }
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {

        await Shell.Current.GoToAsync("..");
    }

    // ==========================================
    // 1. GESTIONE SEZIONE DETTAGLI UTENTE
    // ==========================================
    private void OnDettagliCardTapped(object sender, TappedEventArgs e)
    {
        if (BtnApplicaDettagli.IsVisible) return;

        TxtNomeUtente.Text = LblNomeUtente.Text;

        // 2. Aggiornamento in memoria (per la sessione corrente)
        Sessione.Username = TxtNomeUtente.Text;

        // 3. PERSISTENZA: Salva il nome sul dispositivo (rimane anche se chiudi l'app)
        Preferences.Default.Set("CurrentUser", TxtNomeUtente.Text);

        BtnApplicaDettagli.IsVisible = true;
        ImgEditIcon.IsVisible = true;
        TxtNomeUtente.IsVisible = true;
        LblNomeUtente.IsVisible = false;

        TxtPassword.Text = "";
        TxtPasswordOld.Text = "";
        TxtPassword.IsVisible = true;
        TxtPasswordOld.IsVisible = true;
        LblPassword.IsVisible = false;

    }

    private void OnApplicaDettagliClicked(object sender, EventArgs e)
    {
        

        BtnApplicaDettagli.IsVisible = false;
        ImgEditIcon.IsVisible = false;
        TxtNomeUtente.IsVisible = false;
        LblNomeUtente.IsVisible = true;

        TxtPassword.IsVisible = false;
        TxtPasswordOld.IsVisible = false;
        LblPassword.IsVisible = true;


        if (!string.IsNullOrEmpty(TxtPassword.Text)) LblPassword.Text = "********";


        // 1. Prendi il nuovo valore inserito dall'utente nella Entry (es. un campo di testo chiamato 'NuovoNomeEntry')
        string nuovoNome = TxtNomeUtente.Text;

        if (string.IsNullOrWhiteSpace(nuovoNome) || nuovoNome.Trim() == Sessione.Username)
        {
            return;
        }

        LblNomeUtente.Text = TxtNomeUtente.Text;

        // 2. Chiama il metodo di AuthService che abbiamo appena creato
        bool successo = AuthService.CambiaUsername(nuovoNome);

        if (successo)
        {
            DisplayAlert("Successo", "Il tuo username è stato modificato con successo!", "OK");

            // (Opzionale) Aggiorna un'eventuale Label grafica per mostrare subito il nuovo nome
            // MioNomeLabel.Text = nuovoNome;
        }
        else
        {
            DisplayAlert("Errore", "Impossibile modificare l'username. Potrebbe essere già in uso.", "OK");
        }

        // 1. Recupera i testi inseriti nei campi (sostituisci i nomi con quelli delle tue Entry XAML)
        string vecchiaPass = TxtPasswordOld.Text;
        string nuovaPass = TxtPassword.Text;

        // Controllo di validità dei campi
        if (string.IsNullOrWhiteSpace(vecchiaPass) && string.IsNullOrWhiteSpace(nuovaPass))
        {
            return; // Nessuna modifica richiesta, esci senza fare nulla
        }
        else if (string.IsNullOrWhiteSpace(vecchiaPass) || string.IsNullOrWhiteSpace(nuovaPass))
        {
            DisplayAlert("Errore", "Entrambi i campi password sono obbligatori.", "OK");
            return;
        }

        // 2. Esegui il metodo di modifica passando i due valori
        bool successoP = AuthService.CambiaPassword(vecchiaPass, nuovaPass);

        if (successoP)
        {
            DisplayAlert("Successo", "La tua password è stata aggiornata permanentemente!", "OK");

            // Pulisci i campi grafici per sicurezza
            TxtPasswordOld.Text = string.Empty;
            TxtPassword.Text = string.Empty;
        }
        else
        {
            // Questo errore appare se la vecchia password digitata non corrisponde all'hash memorizzato
            DisplayAlert("Errore", "La vecchia password inserita non è corretta.", "OK");
        }


        
    }
  

    // ==========================================
    // 2. GESTIONE SEZIONE GENERALI
    // ==========================================
    private void OnGeneraliCardTapped(object sender, TappedEventArgs e)
    {
        if (BtnApplicaGenerali.IsVisible) return;

        SwPromemoria.IsToggled = (LblPromemoria.Text == "Abilitato");

        BtnApplicaGenerali.IsVisible = true;
        SwPromemoria.IsVisible = true;
        LblPromemoria.IsVisible = false;
    }

    private void OnApplicaGeneraliClicked(object sender, EventArgs e)
    {
        LblPromemoria.Text = SwPromemoria.IsToggled ? "Abilitato" : "Disabilitato";

        BtnApplicaGenerali.IsVisible = false;
        SwPromemoria.IsVisible = false;
        LblPromemoria.IsVisible = true;
    }

    // ==========================================
    // 3. GESTIONE SEZIONE PERSONALIZZAZIONE
    // ==========================================
    private void OnPersonalizzazioneCardTapped(object sender, TappedEventArgs e)
    {
        if (BtnApplicaPersonalizzazione.IsVisible) return;

        PkrStileColori.SelectedItem = LblStileColori.Text;
        PkrStileFont.SelectedItem = LblStileFont.Text;
        PkrSuonoNotifiche.SelectedItem = LblSuonoNotifiche.Text;

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
        if (PkrStileColori.SelectedItem != null) LblStileColori.Text = PkrStileColori.SelectedItem.ToString();
        if (PkrStileFont.SelectedItem != null) LblStileFont.Text = PkrStileFont.SelectedItem.ToString();
        if (PkrSuonoNotifiche.SelectedItem != null) LblSuonoNotifiche.Text = PkrSuonoNotifiche.SelectedItem.ToString();

        BtnApplicaPersonalizzazione.IsVisible = false;
        PkrStileColori.IsVisible = false;
        PkrStileFont.IsVisible = false;
        PkrSuonoNotifiche.IsVisible = false;
        LblStileColori.IsVisible = true;
        LblStileFont.IsVisible = true;
        LblSuonoNotifiche.IsVisible = true;
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {

        if (Application.Current != null)
        {
            // Usa il percorso assoluto completo della classe Login
            Application.Current.MainPage = new MyAgenda.Login();
        }
    }

}