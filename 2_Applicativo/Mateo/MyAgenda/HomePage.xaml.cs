using System.Text.Json;
using System.Linq;

namespace MyAgenda
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            
        }
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
                "Data selezionata: " + e.NewDate.ToString("dd/MM/yyyy");
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
    }
}