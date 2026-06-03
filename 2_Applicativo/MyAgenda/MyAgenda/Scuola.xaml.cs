using System.Collections.ObjectModel;
using System.Text.Json;

namespace MyAgenda;

public partial class Scuola : ContentPage
{
    ObservableCollection<MateriaModel> primoSemestre =
    new ObservableCollection<MateriaModel>();

    ObservableCollection<MateriaModel> secondoSemestre =
        new ObservableCollection<MateriaModel>();

    ObservableCollection<MateriaModel> materie;

    string filePath =
    Path.Combine(
        FileSystem.AppDataDirectory,
        $"{Sessione.Username}_materie.txt");

    public Scuola()
    {
        
        InitializeComponent();

        CaricaDati();

        materie = primoSemestre;

        MaterieCollection.ItemsSource = materie;
    }

    // AGGIUNGI MATERIA
    private async void OnAddMateriaClicked(object sender, EventArgs e)
    {
        string nome =
            await DisplayPromptAsync("Materia", "Nome materia");

        if (nome != null)
        {
            materie.Add(new MateriaModel
            {
                Nome = nome,
                Media = "",
                Note = new List<double>()
            });

            SalvaDati();
        }
    }
    //Torna alla home
    private async void OnHomeClicked(object sender, EventArgs e)
    {
        SalvaDati();

        await Shell.Current.GoToAsync("//HomePage");
    }

    // AGGIUNGI NOTA
    private async void OnAddNotaClicked(object sender, EventArgs e)
    {
        Button bottone = sender as Button;

        MateriaModel materia =
            bottone.BindingContext as MateriaModel;

        string votoStringa =
            await DisplayPromptAsync("Nota", "Inserisci voto");

        if (double.TryParse(votoStringa, out double voto))
        {
            materia.Note.Add(voto);

            double media =
                materia.Note.Average();

            materia.Media =
                media.ToString("0.0");

            SalvaDati();

            MaterieCollection.ItemsSource = null;
            MaterieCollection.ItemsSource = materie;
        }
    }

    // MODIFICA NOTA
    private async void OnModificaNotaClicked(object sender, EventArgs e)
    {
        Button bottone = sender as Button;

        MateriaModel materia =
            bottone.BindingContext as MateriaModel;

        if (materia.Note.Count == 0)
        {
            await DisplayAlert("Errore",
                "Non ci sono note",
                "OK");

            return;
        }

        string listaNote = "";

        for (int i = 0; i < materia.Note.Count; i++)
        {
            listaNote += $"{i} = {materia.Note[i]}\n";
        }

        string indiceStringa =
            await DisplayPromptAsync(
                "Modifica Nota",
                listaNote + "\nScrivi indice nota");

        if (int.TryParse(indiceStringa, out int indice))
        {
            string nuovoVotoStringa =
                await DisplayPromptAsync(
                    "Nuovo voto",
                    "Inserisci nuovo voto");

            if (double.TryParse(nuovoVotoStringa, out double nuovoVoto))
            {
                materia.Note[indice] = nuovoVoto;

                materia.Media =
                    materia.Note.Average().ToString("0.0");

                SalvaDati();

                MaterieCollection.ItemsSource = null;
                MaterieCollection.ItemsSource = materie;
            }
        }
    }

    // ELIMINA NOTA
    private async void OnEliminaNotaClicked(object sender, EventArgs e)
    {
        Button bottone = sender as Button;

        MateriaModel materia =
            bottone.BindingContext as MateriaModel;

        if (materia.Note.Count == 0)
        {
            await DisplayAlert("Errore",
                "Non ci sono note",
                "OK");

            return;
        }

        string listaNote = "";

        for (int i = 0; i < materia.Note.Count; i++)
        {
            listaNote += $"{i} = {materia.Note[i]}\n";
        }

        string indiceStringa =
            await DisplayPromptAsync(
                "Elimina Nota",
                listaNote + "\nScrivi indice nota");

        if (int.TryParse(indiceStringa, out int indice))
        {
            materia.Note.RemoveAt(indice);

            if (materia.Note.Count > 0)
            {
                materia.Media =
                    materia.Note.Average().ToString("0.0");
            }
            else
            {
                materia.Media = "";
            }

            SalvaDati();

            MaterieCollection.ItemsSource = null;
            MaterieCollection.ItemsSource = materie;
        }
    }

    // ELIMINA MATERIA
    private async void OnEliminaMateriaClicked(object sender, EventArgs e)
    {
        string nome =
            await DisplayPromptAsync(
                "Elimina Materia",
                "Scrivi il nome della materia");

        MateriaModel materiaDaEliminare = null;

        foreach (MateriaModel materia in materie)
        {
            if (materia.Nome == nome)
            {
                materiaDaEliminare = materia;
            }
        }

        if (materiaDaEliminare != null)
        {
            materie.Remove(materiaDaEliminare);

            SalvaDati();
        }
    }

    // MODIFICA MATERIA
    private async void OnModificaMateriaClicked(object sender, EventArgs e)
    {
        string vecchioNome =
            await DisplayPromptAsync(
                "Modifica Materia",
                "Scrivi il nome della materia");

        foreach (MateriaModel materia in materie)
        {
            if (materia.Nome == vecchioNome)
            {
                string nuovoNome =
                    await DisplayPromptAsync(
                        "Nuovo Nome",
                        "Inserisci nuovo nome");

                materia.Nome = nuovoNome;

                SalvaDati();

                MaterieCollection.ItemsSource = null;
                MaterieCollection.ItemsSource = materie;
            }
        }
    }

    // PICKER SEMESTRE
    private void OnSemestreSelected(object sender, EventArgs e)
    {
        string semestre =
            semestrePicker.SelectedItem.ToString();

        selectedSemestreLabel.Text = semestre;

        if (semestre == "Primo semestre")
        {
            materie = primoSemestre;
        }
        else
        {
            materie = secondoSemestre;
        }

        MaterieCollection.ItemsSource = null;
        MaterieCollection.ItemsSource = materie;
    }

    // SALVA FILE
    private void SalvaDati()
    {
        DatiSalvati dati = new DatiSalvati
        {
            PrimoSemestre = primoSemestre,
            SecondoSemestre = secondoSemestre
        };

        string json =
            JsonSerializer.Serialize(dati);

        File.WriteAllText(filePath, json);
    }

    // CARICA FILE
    private void CaricaDati()
    {
        if (File.Exists(filePath))
        {
            string json =
                File.ReadAllText(filePath);

            DatiSalvati dati =
                JsonSerializer.Deserialize<DatiSalvati>(json);

            if (dati != null)
            {
                primoSemestre = dati.PrimoSemestre;
                secondoSemestre = dati.SecondoSemestre;
            }
        }
    }

    // MODEL
    public class MateriaModel
    {
        public string Nome { get; set; }

        public string Media { get; set; }

        public List<double> Note { get; set; }
    }
    public class DatiSalvati
    {
        public ObservableCollection<MateriaModel> PrimoSemestre { get; set; }

        public ObservableCollection<MateriaModel> SecondoSemestre { get; set; }
    }
}
//La pagina contiene prodotti generati dall ai
//Commentato con ai quasi tutto