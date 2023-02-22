using MyShoppingApp.ViewModel;
namespace MyShoppingApp.View;


public partial class MainPage : ContentPage
{

    private MainViewModel _viewModel;

    public MainPage(MainViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    public async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await _viewModel.OnLoaded();
    }
}

