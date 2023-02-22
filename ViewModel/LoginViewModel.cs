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


namespace MyShoppingApp.ViewModel
{
    public partial class LoginViewModel : BaseViewModel
    {
        
        [ObservableProperty]
        User user;

        private DatabaseService _databaseService;

        public LoginViewModel(DatabaseService databaseService)
        {

            _databaseService = databaseService;
        }

        public async Task Login()
        {
            try
            {
                if (User == null)
                {
                    await App.ShowAlert($"User is null!");
                    return;
                }
                if (string.IsNullOrEmpty(User.Username) ||
                    string.IsNullOrEmpty(User.Email) ||
                    string.IsNullOrEmpty(User.FName) ||
                    string.IsNullOrEmpty(User.LName) ||
                    string.IsNullOrEmpty(User.Password))
                {
                    await App.ShowAlert($"Please make sure fields aren't empty!");
                    return;
                }

                // TODO: do validation on fields

                var response = await _databaseService.AddUserAsync(User);
                if (response > 0)
                {
                    await App.ShowAlert($"Success! You can now login.");

                    User = new User();
                    // await Shell.Current.GoToAsync($"..", true);
                }
                else
                {
                    await App.ShowAlert($"Error! please try again!");
                }


            }
            catch (Exception ex)
            {
                await App.ShowAlert($"Error: {ex.Message}");
            }
        }
    }
}
