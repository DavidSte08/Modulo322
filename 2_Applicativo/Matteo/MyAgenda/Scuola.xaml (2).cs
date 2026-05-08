using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using System.Collections.ObjectModel;

namespace MyAgenda;

public partial class Scuola : ContentPage
{
    ObservableCollection<MateriaModel> materie =
        new ObservableCollection<MateriaModel>();

    int counter = 1;

    public Scuola()
    {
        InitializeComponent();

        MaterieCollection.ItemsSource = materie;
    }

    private async void OnAddMateriaClicked(object sender, EventArgs e)
    {
        string nome = await DisplayPromptAsync("Materia", "Nome materia");

        materie.Add(new MateriaModel
        {
            Nome = nome,
            Media = ""
        });
    }

    private async void OnAddNotaClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Nota",
            "Qui puoi aggiungere una nota",
            "OK");
    }

    private async void OnModificaNotaClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Nota",
            "Qui puoi aggiungere una nota",
            "OK");

    }
    

    public class MateriaModel
    {
        public string Nome { get; set; }

        public string Media { get; set; }
    }

    private void OnSemestreSelected(object sender, EventArgs e)
    {
        selectedSemestreLabel.Text = semestrePicker.SelectedItem.ToString();
    }
}
