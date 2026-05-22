
namespace MyAgenda;

public partial class Account : ContentPage
{
	public Account()
	{
        InitializeComponent();
	}
    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

}