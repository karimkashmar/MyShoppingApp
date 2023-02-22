using MyShoppingApp.ViewModel;

namespace MyShoppingApp.View;

public partial class ShoppingPage : ContentPage
{
    private ShoppingViewModel _viewModel;

    public ShoppingPage(ShoppingViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await _viewModel.OnLoaded();
    }
}