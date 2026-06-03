using System.Collections.ObjectModel;
using System.Text.Json;
using MyAgenda.Models;
using MyAgenda.Services;

namespace MyAgenda;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
        CaricaEventiGiorno(DateTime.Today);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CaricaProssimiEventi();
        CaricaTopMaterie();
    }

    private async void OnAccountClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(Account));

    private async void OnAttivitaClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(Attivita));

    private async void OnMaterieClicked(object sender, EventArgs e)
        => await Shell.Current.GoToAsync(nameof(Scuola));

    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        selectedDateLabel.Text = "Data selezionata: " + e.NewDate.ToString("dd/MM/yyyy");
        CaricaEventiGiorno(e.NewDate);
    }

    // ── Top 3 materie ──────────────────────────────────────────────────────

    private void CaricaTopMaterie()
    {
        var dati = StorageService.Load<DatiScuola>("materie");
        if (dati == null) return;

        var top = dati.PrimoSemestre
            .Where(m => !string.IsNullOrEmpty(m.Media))
            .OrderByDescending(m => double.TryParse(m.Media, out double v) ? v : 0)
            .Take(3)
            .ToList();

        Label[] labels = { top1Label, top2Label, top3Label };
        for (int i = 0; i < labels.Length; i++)
        {
            labels[i].Text = i < top.Count
                ? $"{top[i].Nome}\nMedia: {top[i].Media}"
                : "-";
        }
    }

    // ── Prossimi eventi ────────────────────────────────────────────────────

    private void CaricaProssimiEventi()
    {
        var attivita = StorageService.Load<ObservableCollection<AttivitaModel>>("attivita");
        if (attivita == null) return;

        var prossimi = attivita
            .Where(a => a.Data >= DateTime.Now)
            .OrderBy(a => a.Data)
            .Take(3)
            .ToList();

        (Label nome, Label data)[] slots =
        {
            (Evento1Nome, Evento1Data),
            (Evento2Nome, Evento2Data),
            (Evento3Nome, Evento3Data)
        };

        for (int i = 0; i < slots.Length; i++)
        {
            if (i < prossimi.Count)
            {
                slots[i].nome.Text = prossimi[i].Nome;
                slots[i].data.Text = prossimi[i].Data.ToString("dd/MM HH:mm");
            }
            else
            {
                slots[i].nome.Text = "Nessun evento";
                slots[i].data.Text = string.Empty;
            }
        }
    }

    // ── Eventi del giorno selezionato ──────────────────────────────────────

    private void CaricaEventiGiorno(DateTime data)
    {
        EventiGiornoLayout.Children.Clear();

        var attivita = StorageService.Load<ObservableCollection<AttivitaModel>>("attivita");
        if (attivita == null)
        {
            MostraLabelVuota();
            return;
        }

        var eventiGiorno = attivita
            .Where(a => a.Data.Date == data.Date)
            .OrderBy(a => a.Data)
            .ToList();

        if (eventiGiorno.Count == 0)
        {
            MostraLabelVuota();
            return;
        }

        foreach (var evento in eventiGiorno)
        {
            EventiGiornoLayout.Children.Add(CreaCardEvento(evento));
        }
    }

    private void MostraLabelVuota()
    {
        EventiGiornoLayout.Children.Add(new Label
        {
            Text = "Nessun evento",
            HorizontalOptions = LayoutOptions.Center,
            TextColor = Colors.Gray
        });
    }

    private static Border CreaCardEvento(AttivitaModel evento)
    {
        var layout = new VerticalStackLayout { Spacing = 5 };
        layout.Children.Add(new Label { Text = evento.Nome, FontAttributes = FontAttributes.Bold, FontSize = 16 });
        layout.Children.Add(new Label { Text = evento.Tipo, TextColor = Colors.Gray });
        layout.Children.Add(new Label { Text = evento.Data.ToString("dd/MM/yyyy HH:mm"), FontSize = 12 });

        return new Border
        {
            BackgroundColor = Color.FromArgb("#EAEAEA"),
            Stroke = Colors.LightGray,
            StrokeThickness = 1,
            Padding = 10,
            Content = layout
        };
    }
}
