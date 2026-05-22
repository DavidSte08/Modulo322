using System.Collections.ObjectModel;
using System.Text.Json;

namespace MyAgenda;

public partial class Attivita : ContentPage
{
    ObservableCollection<AttivitaModel> attivita =
        new ObservableCollection<AttivitaModel>();

    string filePath =
        Path.Combine(
            FileSystem.AppDataDirectory,
            $"{Sessione.Username}_attivita.txt");

    public Attivita()
    {
        InitializeComponent();

        CaricaDati();

        calendarPicker.Date = DateTime.Today;

        AggiornaCalendario(DateTime.Today);
    }

    // AGGIUNGI EVENTO
    private async void OnAddEventoClicked(object sender, EventArgs e)
    {
        string nome =
            await DisplayPromptAsync(
                "Evento",
                "Nome evento");

        if (string.IsNullOrWhiteSpace(nome))
            return;

        string tipo =
            await DisplayPromptAsync(
                "Tipo",
                "Tipo evento");

        if (string.IsNullOrWhiteSpace(tipo))
            return;

        string dataStringa =
            await DisplayPromptAsync(
                "Data",
                "Formato: 22/05/2026");

        if (string.IsNullOrWhiteSpace(dataStringa))
            return;

        string oraStringa =
            await DisplayPromptAsync(
                "Ora",
                "Formato: 14:30");

        if (string.IsNullOrWhiteSpace(oraStringa))
            return;

        try
        {
            DateTime data =
                DateTime.Parse(
                    dataStringa + " " + oraStringa);

            attivita.Add(new AttivitaModel
            {
                Nome = nome,
                Tipo = tipo,
                Data = data
            });

            SalvaDati();

            AggiornaCalendario(calendarPicker.Date);
        }
        catch
        {
            await DisplayAlert(
                "Errore",
                "Data o ora non valida",
                "OK");
        }
    }

    // CREA CALENDARIO
    private void AggiornaCalendario(DateTime dataBase)
    {
        CalendarioGrid.Children.Clear();

        CalendarioGrid.RowDefinitions.Clear();
        CalendarioGrid.ColumnDefinitions.Clear();

        // COLONNA ORARI
        CalendarioGrid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = 70
            });

        // 7 GIORNI
        for (int i = 0; i < 7; i++)
        {
            CalendarioGrid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = 120
                });
        }

        // HEADER
        CalendarioGrid.RowDefinitions.Add(
            new RowDefinition
            {
                Height = 40
            });

        // RIGHE ORARI
        for (int i = 0; i < 18; i++)
        {
            CalendarioGrid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = 60
                });
        }

        // HEADER GIORNI
        for (int i = 0; i < 7; i++)
        {
            DateTime giorno =
                dataBase.AddDays(i);

            Border header = new Border
            {
                BackgroundColor =
                    Color.FromArgb("#BDBDBD"),

                Content = new Label
                {
                    Text = giorno.ToString("ddd dd"),
                    TextColor = Colors.White,
                    HorizontalOptions =
                        LayoutOptions.Center,

                    VerticalOptions =
                        LayoutOptions.Center,

                    FontAttributes =
                        FontAttributes.Bold
                }
            };

            CalendarioGrid.Add(header, i + 1, 0);
        }

        // ORARI
        for (int i = 0; i < 18; i++)
        {
            int ora = i + 6;

            Label oraLabel = new Label
            {
                Text = ora + ":00",
                FontSize = 12,
                HorizontalOptions =
                    LayoutOptions.Center,

                VerticalOptions =
                    LayoutOptions.Center
            };

            CalendarioGrid.Add(oraLabel, 0, i + 1);
        }

        // EVENTI
        foreach (AttivitaModel evento in attivita)
        {
            int giorno =
                (evento.Data.Date - dataBase.Date).Days;

            if (giorno >= 0 && giorno < 7)
            {
                int riga =
                    evento.Data.Hour - 5;

                if (riga > 0 && riga < 19)
                {
                    VerticalStackLayout layout =
                        new VerticalStackLayout
                        {
                            Spacing = 2,
                            Padding = 5
                        };

                    layout.Children.Add(new Label
                    {
                        Text = evento.Nome,
                        FontSize = 11,
                        TextColor = Colors.White,
                        HorizontalOptions =
                            LayoutOptions.Center
                    });

                    layout.Children.Add(new Label
                    {
                        Text = evento.Tipo,
                        FontSize = 10,
                        TextColor = Colors.White,
                        HorizontalOptions =
                            LayoutOptions.Center
                    });

                    Border box = new Border
                    {
                        BackgroundColor =
                            Color.FromArgb("#666666"),

                        StrokeThickness = 0,

                        Content = layout
                    };

                    CalendarioGrid.Add(
                        box,
                        giorno + 1,
                        riga);
                }
            }
        }
    }

    // DATE PICKER
    private void OnDateSelected(
        object sender,
        DateChangedEventArgs e)
    {
        AggiornaCalendario(e.NewDate);
    }

    // HOME
    private async void OnHomeClicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("//HomePage");
    }

    // SALVA FILE
    private void SalvaDati()
    {
        string json =
            JsonSerializer.Serialize(attivita);

        File.WriteAllText(filePath, json);
    }

    // CARICA FILE
    private void CaricaDati()
    {
        if (File.Exists(filePath))
        {
            string json =
                File.ReadAllText(filePath);

            ObservableCollection<AttivitaModel> dati =
                JsonSerializer.Deserialize
                <ObservableCollection<AttivitaModel>>
                (json);

            if (dati != null)
            {
                attivita = dati;
            }
        }
    }
}