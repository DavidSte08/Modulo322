using System.Collections.ObjectModel;
using MyAgenda.Models;
using MyAgenda.Services;

namespace MyAgenda;

public partial class Scuola : ContentPage
{
    private ObservableCollection<MateriaModel> _primoSemestre = new();
    private ObservableCollection<MateriaModel> _secondoSemestre = new();
    private ObservableCollection<MateriaModel> _materieCorrente;

    public Scuola()
    {
        InitializeComponent();
        CaricaDati();
        _materieCorrente = _primoSemestre;
        MaterieCollection.ItemsSource = _materieCorrente;
    }

    // ── Navigazione ────────────────────────────────────────────────────────

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        SalvaDati();
        await Shell.Current.GoToAsync("//HomePage");
    }

    // ── CRUD Materie ───────────────────────────────────────────────────────

    private async void OnAddMateriaClicked(object sender, EventArgs e)
    {
        string? nome = await DisplayPromptAsync("Materia", "Nome materia:");
        if (string.IsNullOrWhiteSpace(nome)) return;

        _materieCorrente.Add(new MateriaModel { Nome = nome.Trim() });
        SalvaDati();
    }

    private async void OnModificaMateriaClicked(object sender, EventArgs e)
    {
        string? vecchioNome = await DisplayPromptAsync("Modifica Materia", "Nome materia da modificare:");
        if (string.IsNullOrWhiteSpace(vecchioNome)) return;

        var materia = _materieCorrente.FirstOrDefault(m =>
            m.Nome.Equals(vecchioNome.Trim(), StringComparison.OrdinalIgnoreCase));

        if (materia == null)
        {
            await DisplayAlert("Errore", "Materia non trovata.", "OK");
            return;
        }

        string? nuovoNome = await DisplayPromptAsync("Nuovo Nome", "Inserisci nuovo nome:", initialValue: materia.Nome);
        if (string.IsNullOrWhiteSpace(nuovoNome)) return;

        materia.Nome = nuovoNome.Trim();
        SalvaDati();
        AggiornaColla();
    }

    private async void OnEliminaMateriaClicked(object sender, EventArgs e)
    {
        string? nome = await DisplayPromptAsync("Elimina Materia", "Nome materia da eliminare:");
        if (string.IsNullOrWhiteSpace(nome)) return;

        var materia = _materieCorrente.FirstOrDefault(m =>
            m.Nome.Equals(nome.Trim(), StringComparison.OrdinalIgnoreCase));

        if (materia == null)
        {
            await DisplayAlert("Errore", "Materia non trovata.", "OK");
            return;
        }

        bool conferma = await DisplayAlert("Conferma", $"Eliminare '{materia.Nome}'?", "Sì", "No");
        if (!conferma) return;

        _materieCorrente.Remove(materia);
        SalvaDati();
    }

    // ── CRUD Note ──────────────────────────────────────────────────────────

    private async void OnAddNotaClicked(object sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: MateriaModel materia }) return;

        string? votoStr = await DisplayPromptAsync("Nota", "Inserisci voto:", keyboard: Keyboard.Numeric);
        if (!double.TryParse(votoStr?.Replace(',', '.'), System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out double voto))
        {
            await DisplayAlert("Errore", "Voto non valido.", "OK");
            return;
        }

        if (voto < 0 || voto > 10)
        {
            await DisplayAlert("Errore", "Il voto deve essere tra 0 e 10.", "OK");
            return;
        }

        materia.Note.Add(voto);
        materia.RicalcolaMedia();
        SalvaDati();
        AggiornaColla();
    }

    private async void OnModificaNotaClicked(object sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: MateriaModel materia }) return;

        if (materia.Note.Count == 0)
        {
            await DisplayAlert("Info", "Nessuna nota da modificare.", "OK");
            return;
        }

        string listaNote = string.Join("\n",
            materia.Note.Select((n, i) => $"{i} → {n}"));

        string? indiceStr = await DisplayPromptAsync("Modifica Nota",
            listaNote + "\n\nIndice nota da modificare:", keyboard: Keyboard.Numeric);

        if (!int.TryParse(indiceStr, out int indice) || indice < 0 || indice >= materia.Note.Count)
        {
            await DisplayAlert("Errore", "Indice non valido.", "OK");
            return;
        }

        string? nuovoVotoStr = await DisplayPromptAsync("Nuovo Voto", "Inserisci nuovo voto:",
            initialValue: materia.Note[indice].ToString(), keyboard: Keyboard.Numeric);

        if (!double.TryParse(nuovoVotoStr?.Replace(',', '.'), System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out double nuovoVoto))
        {
            await DisplayAlert("Errore", "Voto non valido.", "OK");
            return;
        }

        materia.Note[indice] = nuovoVoto;
        materia.RicalcolaMedia();
        SalvaDati();
        AggiornaColla();
    }

    private async void OnEliminaNotaClicked(object sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: MateriaModel materia }) return;

        if (materia.Note.Count == 0)
        {
            await DisplayAlert("Info", "Nessuna nota da eliminare.", "OK");
            return;
        }

        string listaNote = string.Join("\n",
            materia.Note.Select((n, i) => $"{i} → {n}"));

        string? indiceStr = await DisplayPromptAsync("Elimina Nota",
            listaNote + "\n\nIndice nota da eliminare:", keyboard: Keyboard.Numeric);

        if (!int.TryParse(indiceStr, out int indice) || indice < 0 || indice >= materia.Note.Count)
        {
            await DisplayAlert("Errore", "Indice non valido.", "OK");
            return;
        }

        materia.Note.RemoveAt(indice);
        materia.RicalcolaMedia();
        SalvaDati();
        AggiornaColla();
    }

    // ── Semestre Picker ────────────────────────────────────────────────────

    private void OnSemestreSelected(object sender, EventArgs e)
    {
        if (semestrePicker.SelectedItem?.ToString() is not string semestre) return;

        selectedSemestreLabel.Text = semestre;
        _materieCorrente = semestre == "Primo semestre" ? _primoSemestre : _secondoSemestre;
        AggiornaColla();
    }

    // ── Persistenza ────────────────────────────────────────────────────────

    private void SalvaDati()
    {
        StorageService.Save("materie", new DatiScuola
        {
            PrimoSemestre = _primoSemestre,
            SecondoSemestre = _secondoSemestre
        });
    }

    private void CaricaDati()
    {
        var dati = StorageService.Load<DatiScuola>("materie");
        if (dati != null)
        {
            _primoSemestre = dati.PrimoSemestre;
            _secondoSemestre = dati.SecondoSemestre;
        }
    }

    private void AggiornaColla()
    {
        MaterieCollection.ItemsSource = null;
        MaterieCollection.ItemsSource = _materieCorrente;
    }
}
