namespace MyAgenda;

public partial class Attivita : ContentPage
{
	public Attivita()
	{
		InitializeComponent();
	}
    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}