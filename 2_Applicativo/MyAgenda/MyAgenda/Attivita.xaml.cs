using System.Collections.ObjectModel;
using System.Text.Json;

namespace MyAgenda;

public partial class Attivita : ContentPage
{
    ObservableCollection<AttivitaModel> attivita =
        new ObservableCollection<AttivitaModel>();

    List<string> nomiPrecedenti =
        new List<string>();

    List<string> tipiPrecedenti = new List<string>();

    readonly List<string> tipiDisponibili =
        new()
        {
        "Scuola",
        "Sport",
        "Lavoro",
        "Personale",
        "Importante",
        "Altro"
        };

    string filePath =
        Path.Combine(
            FileSystem.AppDataDirectory,
            $"{Sessione.Username}_attivita.txt");

    public Attivita()
    {
        InitializeComponent();

        tipoPicker.ItemsSource = tipiDisponibili;

        CaricaDati();

        AggiornaNomi();

        calendarPicker.Date = DateTime.Today;

        nuovaDataPicker.Date = DateTime.Today;

        AggiornaCalendario(DateTime.Today);
    }

    private void AggiornaNomi()
    {
        nomiPrecedenti.Clear();

        foreach (AttivitaModel a in attivita)
        {
            if (!nomiPrecedenti.Contains(a.Nome))
            {
                nomiPrecedenti.Add(a.Nome);
            }
        }

        nomePicker.ItemsSource = null;
        nomePicker.ItemsSource = nomiPrecedenti;
    }
    private Color OttieniColoreEvento(string tipo)
    {
        switch (tipo)
        {
            case "Scuola":
                return Color.FromArgb("#2563EB");

            case "Sport":
                return Color.FromArgb("#16A34A");

            case "Lavoro":
                return Color.FromArgb("#EA580C");

            case "Personale":
                return Color.FromArgb("#7C3AED");

            case "Importante":
                return Color.FromArgb("#DC2626");

            default:
                return Color.FromArgb("#6B7280");
        }
    }
    // AGGIUNGI EVENTO
    private async void OnAddEventoClicked(object sender, EventArgs e)
    {
        string nome = "";

        if (!string.IsNullOrWhiteSpace(nomeEntry.Text))
        {
            nome = nomeEntry.Text.Trim();
        }
        else if (nomePicker.SelectedItem != null)
        {
            nome = nomePicker.SelectedItem.ToString();
        }

        if (string.IsNullOrWhiteSpace(nome))
        {
            await DisplayAlert(
                "Errore",
                "Inserisci o seleziona un nome",
                "OK");

            return;
        }

        if (tipoPicker.SelectedItem == null)
        {
            await DisplayAlert(
                "Errore",
                "Seleziona un tipo",
                "OK");

            return;
        }

        string tipo =
            tipoPicker.SelectedItem.ToString();

        DateTime data =
            nuovaDataPicker.Date +
            nuovaOraPicker.Time;

        attivita.Add(new AttivitaModel
        {
            Nome = nome,
            Tipo = tipo,
            Data = data
        });

        SalvaDati();

        AggiornaNomi();

        AggiornaCalendario(calendarPicker.Date);

        nomeEntry.Text = "";

        await DisplayAlert(
            "Successo",
            "Evento creato",
            "OK");
    }
    

    // CALENDARIO
    private void AggiornaCalendario(DateTime dataBase)
    {
        CalendarioGrid.Children.Clear();
        HeaderGrid.Children.Clear();

        CalendarioGrid.RowDefinitions.Clear();
        CalendarioGrid.ColumnDefinitions.Clear();

        HeaderGrid.RowDefinitions.Clear();
        HeaderGrid.ColumnDefinitions.Clear();

        // HEADER
        HeaderGrid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = 70
            });

        for (int i = 0; i < 7; i++)
        {
            HeaderGrid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = 140
                    
                });
        }

        HeaderGrid.RowDefinitions.Add(
            new RowDefinition
            {
                Height = 40
            });

        // GRID
        CalendarioGrid.ColumnDefinitions.Add(
            new ColumnDefinition
            {
                Width = 70
            });

        for (int i = 0; i < 7; i++)
        {
            CalendarioGrid.ColumnDefinitions.Add(
                new ColumnDefinition
                {
                    Width = 140
                });
        }

        for (int i = 0; i < 24; i++)
        {
            CalendarioGrid.RowDefinitions.Add(
                new RowDefinition
                {
                    Height = 50
                });
        }

        // GIORNI
        for (int i = 0; i < 7; i++)
        {
            DateTime giorno =
                dataBase.AddDays(i);

            Border header = new Border
            {
                BackgroundColor =
                    Color.FromArgb("#BDBDBD"),
                Stroke = Colors.Black,
                StrokeThickness = 1,

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

            HeaderGrid.Add(header, i + 1, 0);
        }

        // ORARI
        for (int i = 0; i < 24; i++)
        {
            Border oraBorder = new Border
            {
                Stroke = Colors.LightGray,
                StrokeThickness = 0.5,

                Content = new Label
                {
                    Text = i + ":00",

                    FontSize = 12,

                    HorizontalOptions =
            LayoutOptions.Center,

                    VerticalOptions =
            LayoutOptions.Center
                }
            };

            CalendarioGrid.Add(
                oraBorder,
                0,
                i);
        }
        // CELLE VUOTE CON LINEE
        for (int r = 0; r < 24; r++)
        {
            for (int c = 1; c <= 7; c++)
            {
                Border cella = new Border
                {
                    Stroke = Colors.LightGray,
                    StrokeThickness = 0.5
                };

                CalendarioGrid.Add(cella, c, r);
            }
        }
        // EVENTI
        foreach (AttivitaModel evento in attivita)
        {
            int giorno =
                (evento.Data.Date - dataBase.Date).Days;

            if (giorno >= 0 && giorno < 7)
            {
                int riga =
                    evento.Data.Hour;

                if (riga >= 0 && riga < 24)
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

                    layout.Children.Add(new Label
                    {
                        Text = evento.Data.ToString("HH:mm"),
                        FontSize = 10,
                        TextColor = Colors.White,

                        HorizontalOptions =
                            LayoutOptions.Center
                    });

                    Border box = new Border
                    {
                        BackgroundColor =
    OttieniColoreEvento(evento.Tipo),

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

    // SCROLL HEADER
    private void BodyScroll_Scrolled(
        object sender,
        ScrolledEventArgs e)
    {
        HeaderScroll.ScrollToAsync(
            e.ScrollX,
            0,
            false);
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

    // SALVA
    private void SalvaDati()
    {
        string json =
            JsonSerializer.Serialize(attivita);

        File.WriteAllText(filePath, json);
    }

    // CARICA
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