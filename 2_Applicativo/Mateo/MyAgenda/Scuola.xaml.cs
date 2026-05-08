using System.Collections.ObjectModel;

namespace MyAgenda;

public partial class Scuola : ContentPage
{
    ObservableCollection<MateriaModel> materie =
        new ObservableCollection<MateriaModel>();

    public Scuola()
    {
        InitializeComponent();

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
        }
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

            // aggiorna la CollectionView
            MaterieCollection.ItemsSource = null;
            MaterieCollection.ItemsSource = materie;
        }
    }

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

                MaterieCollection.ItemsSource = null;
                MaterieCollection.ItemsSource = materie;
            }
        }
    }
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

            MaterieCollection.ItemsSource = null;
            MaterieCollection.ItemsSource = materie;
        }
    }
    // MODEL
    public class MateriaModel
    {
        public string Nome { get; set; }

        public string Media { get; set; }

        public List<double> Note { get; set; }
    }

    // PICKER SEMESTRE
    private void OnSemestreSelected(object sender, EventArgs e)
    {
        selectedSemestreLabel.Text =
            semestrePicker.SelectedItem.ToString();
    }

    private void OnEliminaMateriaClicked(object sender, EventArgs e)
    {

    }
}