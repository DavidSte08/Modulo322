
namespace MyAgenda;

public partial class Account : ContentPage
{
    string[] profilePictures = new string[]
    {
        "melodie.png",
        "superteo08_logo.png",
        "viggo2.png",
        "juggy.png",
        "haku.png",
        "st08brawl.png",
        "girlfriend.jpeg"
    };
    public Account()
	{
        InitializeComponent();

	}
    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

}