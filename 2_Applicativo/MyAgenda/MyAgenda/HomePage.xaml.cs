using System.Text.Json;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text.Json;
namespace MyAgenda
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            CaricaProssimiEventi();

            CaricaEventiGiorno(DateTime.Today);
        }
        string filePath =
            Path.Combine(
            FileSystem.AppDataDirectory,
            $"{Sessione.Username}_attivita.txt");
        protected override void OnAppearing()
        {
            base.OnAppearing();

            CaricaTopMaterie();
        }

        private async void OnAccountClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Account");
        }

        private async void OnAttivitaClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Attivita");
        }

        private async void OnMaterieClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("Scuola");
        }

        private void OnDateSelected(object sender, DateChangedEventArgs e)
        {
            selectedDateLabel.Text =
                "Data selezionata: " +
                e.NewDate.ToString("dd/MM/yyyy");

            CaricaEventiGiorno(e.NewDate);
        }

        // TOP 3 MATERIE
        private void CaricaTopMaterie()
        {
            string filePath =
    Path.Combine(
        FileSystem.AppDataDirectory,
        $"{Sessione.Username}_materie.txt");

            if (!File.Exists(filePath))
                return;

            string json =
                File.ReadAllText(filePath);

            DatiSalvati dati =
                JsonSerializer.Deserialize<DatiSalvati>(json);

            if (dati == null)
                return;

            // PRENDE IL PRIMO SEMESTRE
            List<MateriaModel> lista =
                dati.PrimoSemestre;

            var top =
                lista.OrderByDescending(x =>
                {
                    if (x.Media == "")
                        return 0;

                    return double.Parse(x.Media);
                }).Take(3).ToList();

            if (top.Count > 0)
            {
                top1Label.Text =
                    $"{top[0].Nome}\nMedia: {top[0].Media}";
            }

            if (top.Count > 1)
            {
                top2Label.Text =
                    $"{top[1].Nome}\nMedia: {top[1].Media}";
            }

            if (top.Count > 2)
            {
                top3Label.Text =
                    $"{top[2].Nome}\nMedia: {top[2].Media}";
            }
        }
        private void CaricaProssimiEventi()
        {
            if (!File.Exists(filePath))
                return;

            string json =
                File.ReadAllText(filePath);

            ObservableCollection<AttivitaModel> attivita =
                JsonSerializer.Deserialize
                <ObservableCollection<AttivitaModel>>
                (json);

            if (attivita == null)
                return;

            List<AttivitaModel> prossimi =
                attivita
                .Where(a => a.Data >= DateTime.Now)
                .OrderBy(a => a.Data)
                .Take(3)
                .ToList();

            if (prossimi.Count > 0)
            {
                Evento1Nome.Text =
                    prossimi[0].Nome;

                Evento1Data.Text =
                    prossimi[0].Data.ToString(
                        "dd/MM HH:mm");
            }

            if (prossimi.Count > 1)
            {
                Evento2Nome.Text =
                    prossimi[1].Nome;

                Evento2Data.Text =
                    prossimi[1].Data.ToString(
                        "dd/MM HH:mm");
            }

            if (prossimi.Count > 2)
            {
                Evento3Nome.Text =
                    prossimi[2].Nome;

                Evento3Data.Text =
                    prossimi[2].Data.ToString(
                        "dd/MM HH:mm");
            }
        }
        private void CaricaEventiGiorno(DateTime data)
        {
            EventiGiornoLayout.Children.Clear();

            if (!File.Exists(filePath))
                return;

            string json =
                File.ReadAllText(filePath);

            ObservableCollection<AttivitaModel> attivita =
                JsonSerializer.Deserialize
                <ObservableCollection<AttivitaModel>>
                (json);

            if (attivita == null)
                return;

            List<AttivitaModel> eventiGiorno =
                attivita
                .Where(a => a.Data.Date == data.Date)
                .OrderBy(a => a.Data)
                .ToList();

            if (eventiGiorno.Count == 0)
            {
                EventiGiornoLayout.Children.Add(
                    new Label
                    {
                        Text = "Nessun evento",
                        HorizontalOptions =
                            LayoutOptions.Center
                    });

                return;
            }

            foreach (AttivitaModel evento in eventiGiorno)
            {
                Border box = new Border
                {
                    BackgroundColor =
                        Color.FromArgb("#EAEAEA"),

                    Stroke = Colors.Gray,

                    StrokeThickness = 1,

                    Padding = 10
                };

                VerticalStackLayout layout =
                    new VerticalStackLayout
                    {
                        Spacing = 5
                    };

                layout.Children.Add(new Label
                {
                    Text = evento.Nome,
                    FontAttributes =
                        FontAttributes.Bold,

                    FontSize = 16
                });

                layout.Children.Add(new Label
                {
                    Text = evento.Tipo
                });

                layout.Children.Add(new Label
                {
                    Text =
                        evento.Data.ToString(
                            "dd/MM/yyyy HH:mm")
                });

                box.Content = layout;

                EventiGiornoLayout.Children.Add(box);
            }
        }

        // CLASSI
        public class MateriaModel
        {
            public string Nome { get; set; }

            public string Media { get; set; }

            public List<double> Note { get; set; }
        }

        public class DatiSalvati
        {
            public List<MateriaModel> PrimoSemestre { get; set; }

            public List<MateriaModel> SecondoSemestre { get; set; }
        }
        public class AttivitaModel
        {
            public string Nome { get; set; }

            public string Tipo { get; set; }

            public DateTime Data { get; set; }
        }
    }
    
}