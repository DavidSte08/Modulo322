using System.Collections.ObjectModel;
using MyAgenda.Models;
using MyAgenda.Services;

namespace MyAgenda;

public partial class Attivita : ContentPage
{
    private ObservableCollection<AttivitaModel> _attivita = new();
    private readonly List<string> _nomiPrecedenti = new();
    private readonly List<string> _tipiPrecedenti = new();

    public Attivita()
    {
        InitializeComponent();
        CaricaDati();
        AggiornaQuickAdd();
        calendarPicker.Date = DateTime.Today;
        quickDatePicker.Date = DateTime.Today;
        AggiornaCalendario(DateTime.Today);
    }

    // ── Aggiungi Evento ────────────────────────────────────────────────────

    private async void OnAddEventoClicked(object sender, EventArgs e)
    {
        string? nome = await DisplayPromptAsync("Evento", "Nome evento:");
        if (string.IsNullOrWhiteSpace(nome)) return;

        string? tipo = await DisplayPromptAsync("Tipo", "Tipo evento:");
        if (string.IsNullOrWhiteSpace(tipo)) return;

        string? dataStr = await DisplayPromptAsync("Data", "Formato: gg/mm/aaaa");
        if (string.IsNullOrWhiteSpace(dataStr)) return;

        string? oraStr = await DisplayPromptAsync("Ora", "Formato: hh:mm");
        if (string.IsNullOrWhiteSpace(oraStr)) return;

        if (!DateTime.TryParse($"{dataStr} {oraStr}", out DateTime data))
        {
            await DisplayAlert("Errore", "Data o ora non valida. Usa il formato corretto.", "OK");
            return;
        }

        _attivita.Add(new AttivitaModel { Nome = nome.Trim(), Tipo = tipo.Trim(), Data = data });
        SalvaEAggiorna();
    }

    // ── Quick Add ──────────────────────────────────────────────────────────

    private async void OnQuickAddClicked(object sender, EventArgs e)
    {
        if (quickNomePicker.SelectedItem == null || quickTipoPicker.SelectedItem == null)
        {
            await DisplayAlert("Errore", "Seleziona nome e tipo dall'elenco.", "OK");
            return;
        }

        _attivita.Add(new AttivitaModel
        {
            Nome = quickNomePicker.SelectedItem.ToString()!,
            Tipo = quickTipoPicker.SelectedItem.ToString()!,
            Data = quickDatePicker.Date + quickTimePicker.Time
        });

        SalvaEAggiorna();
    }

    private void AggiornaQuickAdd()
    {
        _nomiPrecedenti.Clear();
        _tipiPrecedenti.Clear();

        foreach (var a in _attivita)
        {
            if (!_nomiPrecedenti.Contains(a.Nome)) _nomiPrecedenti.Add(a.Nome);
            if (!_tipiPrecedenti.Contains(a.Tipo)) _tipiPrecedenti.Add(a.Tipo);
        }

        quickNomePicker.ItemsSource = null;
        quickTipoPicker.ItemsSource = null;
        quickNomePicker.ItemsSource = _nomiPrecedenti;
        quickTipoPicker.ItemsSource = _tipiPrecedenti;
    }

    // ── Calendario ─────────────────────────────────────────────────────────

    private void AggiornaCalendario(DateTime dataBase)
    {
        CalendarioGrid.Children.Clear();
        HeaderGrid.Children.Clear();
        CalendarioGrid.RowDefinitions.Clear();
        CalendarioGrid.ColumnDefinitions.Clear();
        HeaderGrid.RowDefinitions.Clear();
        HeaderGrid.ColumnDefinitions.Clear();

        // Colonna ore
        HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 70 });
        CalendarioGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 70 });

        for (int i = 0; i < 7; i++)
        {
            HeaderGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 140 });
            CalendarioGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = 140 });
        }

        HeaderGrid.RowDefinitions.Add(new RowDefinition { Height = 40 });

        for (int i = 0; i < 24; i++)
            CalendarioGrid.RowDefinitions.Add(new RowDefinition { Height = 50 });

        // Intestazioni giorni
        for (int i = 0; i < 7; i++)
        {
            HeaderGrid.Add(new Border
            {
                BackgroundColor = Color.FromArgb("#BDBDBD"),
                Stroke = Colors.Black,
                StrokeThickness = 1,
                Content = new Label
                {
                    Text = dataBase.AddDays(i).ToString("ddd dd"),
                    TextColor = Colors.White,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold
                }
            }, i + 1, 0);
        }

        // Orari e celle
        for (int r = 0; r < 24; r++)
        {
            CalendarioGrid.Add(new Border
            {
                Stroke = Colors.LightGray,
                StrokeThickness = 0.5,
                Content = new Label
                {
                    Text = $"{r}:00",
                    FontSize = 12,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            }, 0, r);

            for (int c = 1; c <= 7; c++)
                CalendarioGrid.Add(new Border { Stroke = Colors.LightGray, StrokeThickness = 0.5 }, c, r);
        }

        // Posiziona eventi
        foreach (var evento in _attivita)
        {
            int giorno = (evento.Data.Date - dataBase.Date).Days;
            if (giorno < 0 || giorno >= 7) continue;

            int riga = evento.Data.Hour;
            if (riga < 0 || riga >= 24) continue;

            var layout = new VerticalStackLayout { Spacing = 2, Padding = 5 };
            layout.Children.Add(new Label { Text = evento.Nome, FontSize = 11, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center });
            layout.Children.Add(new Label { Text = evento.Tipo, FontSize = 10, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center });
            layout.Children.Add(new Label { Text = evento.Data.ToString("HH:mm"), FontSize = 10, TextColor = Colors.White, HorizontalOptions = LayoutOptions.Center });

            CalendarioGrid.Add(new Border
            {
                BackgroundColor = Color.FromArgb("#666666"),
                StrokeThickness = 0,
                Content = layout
            }, giorno + 1, riga);
        }
    }

    private void BodyScroll_Scrolled(object sender, ScrolledEventArgs e)
        => HeaderScroll.ScrollToAsync(e.ScrollX, 0, false);

    private void OnDateSelected(object sender, DateChangedEventArgs e)
        => AggiornaCalendario(e.NewDate);

    private async void OnHomeClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync("//HomePage");

    // ── Persistenza ────────────────────────────────────────────────────────

    private void SalvaEAggiorna()
    {
        SalvaDati();
        AggiornaQuickAdd();
        AggiornaCalendario(calendarPicker.Date);
    }

    private void SalvaDati()
        => StorageService.Save("attivita", _attivita);

    private void CaricaDati()
    {
        var dati = StorageService.Load<ObservableCollection<AttivitaModel>>("attivita");
        if (dati != null) _attivita = dati;
    }
}
