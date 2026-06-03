using System.Collections.ObjectModel;

namespace MyAgenda.Models;

/// <summary>
/// Rappresenta una materia scolastica con voti e media.
/// </summary>
public class MateriaModel
{
    public string Nome { get; set; } = string.Empty;
    public string Media { get; set; } = string.Empty;
    public List<double> Note { get; set; } = new();

    /// <summary>
    /// Ricalcola la media dai voti presenti.
    /// </summary>
    public void RicalcolaMedia()
    {
        Media = Note.Count > 0
            ? Note.Average().ToString("0.0")
            : string.Empty;
    }
}

/// <summary>
/// Contenitore per i dati di entrambi i semestri.
/// </summary>
public class DatiScuola
{
    public ObservableCollection<MateriaModel> PrimoSemestre { get; set; } = new();
    public ObservableCollection<MateriaModel> SecondoSemestre { get; set; } = new();
}

/// <summary>
/// Rappresenta un'attività/evento con nome, tipo e data.
/// </summary>
public class AttivitaModel
{
    public string Nome { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public DateTime Data { get; set; }
}
