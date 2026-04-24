namespace MyAgenda;

public partial class Scuola : ContentPage
{
	public Scuola()
	{
		InitializeComponent();
	}
    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}