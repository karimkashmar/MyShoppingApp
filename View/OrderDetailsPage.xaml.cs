using MyShoppingApp.ViewModel;

namespace MyShoppingApp.View;

public partial class OrderDetailsPage : ContentPage
{
    private OrderDetailsViewModel _viewModel;
    public OrderDetailsPage(OrderDetailsViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    public async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await _viewModel.OnLoaded();
    }

    public async void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        await _viewModel.OnRequestedAmountChanged();
    }
}