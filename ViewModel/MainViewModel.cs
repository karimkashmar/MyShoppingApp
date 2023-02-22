using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using MyShoppingApp.Services;
using MyShoppingApp.View;

namespace MyShoppingApp.ViewModel
{
    public partial class MainViewModel : BaseViewModel
    {
        private DatabaseService _databaseService;

        public MainViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        [RelayCommand]
        public async Task Login()
        {
            await Shell.Current.GoToAsync($"{nameof(LoginPage)}", true);

        }
        [RelayCommand]
        public async Task Register()
        {

            await Shell.Current.GoToAsync($"{nameof(RegisterPage)}", true);

        }

        public async Task OnLoaded()
        {
            if(await _databaseService.InitializeDatabaseAsync())
            {
                await App.ShowAlert("Database Initialization Success!");

            }
            else
            {
                await App.ShowAlert("Database Initialization Failed!");
            }

        }
    }
}
