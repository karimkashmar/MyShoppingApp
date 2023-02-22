namespace MyShoppingApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

	public static async Task ShowAlert(string message)
	{
		await Shell.Current.DisplayAlert("Alert", message, "OK");
	}

}
