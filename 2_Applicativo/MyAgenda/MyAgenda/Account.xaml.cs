namespace MyAgenda;

public partial class Account : ContentPage
{
    public Account()
    {

        InitializeComponent();
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

        BtnApplicaDettagli.IsVisible = true;
        ImgEditIcon.IsVisible = true;
        TxtNomeUtente.IsVisible = true;
        LblNomeUtente.IsVisible = false;

        TxtPassword.Text = "";
        TxtPassword.IsVisible = true;
        LblPassword.IsVisible = false;

    }

    private void OnApplicaDettagliClicked(object sender, EventArgs e)
    {
        LblNomeUtente.Text = TxtNomeUtente.Text;

        BtnApplicaDettagli.IsVisible = false;
        ImgEditIcon.IsVisible = false;
        TxtNomeUtente.IsVisible = false;
        LblNomeUtente.IsVisible = true;

        if (!string.IsNullOrEmpty(TxtPassword.Text)) LblPassword.Text = "********";

        TxtPassword.IsVisible = false;
        LblPassword.IsVisible = true;
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