using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyShoppingApp.Model;
using MyShoppingApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShoppingApp.Services;
using System.Text.Json;

namespace MyShoppingApp.ViewModel
{
    // view model responsible to login existing users by validating passwords using db.ValidatePasswordAsync
    public partial class LoginViewModel : BaseViewModel
    {

        [ObservableProperty]
        string username;

        [ObservableProperty]
        string password;


        private DatabaseService _databaseService;

        public LoginViewModel(DatabaseService databaseService)
        {

            _databaseService = databaseService;
        }
        [RelayCommand]
        public async Task Login()
        {
            try
            {

                if (string.IsNullOrEmpty(Username) ||
                    string.IsNullOrEmpty(Password))
                {
                    await App.ShowAlert($"Please make sure fields aren't empty!");
                    return;
                }

                // TODO: do validation on fields

                var foundUser = await _databaseService.ValidatePasswordAsync(Username, Password);
                if (foundUser == null)
                {
                    await App.ShowAlert($"Error: username or password invalid.");
                    return;
                }
                else
                {
                    foundUser.Password = null;
                    await Shell.Current.GoToAsync(nameof(DashboardPage), true, new Dictionary<string, object>
                    {
                        {"MyUser", foundUser }
                    });
                    // await Shell.Current.GoToAsync($"{nameof(DashboardPage)}?user={JsonSerializer.Serialize(foundUser)}", true);
                }
            }
            catch (Exception ex)
            {
                await App.ShowAlert($"Error: {ex.Message}");
            }
        }
    }
}
