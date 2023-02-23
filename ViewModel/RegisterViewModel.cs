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
    // view model responsible to register new users 

    public partial class RegisterViewModel : BaseViewModel
    {
        #region Properties
        [ObservableProperty]
        User userObj;

        private DatabaseService _databaseService;
        #endregion

        #region Ctor

        public RegisterViewModel(DatabaseService databaseService)
        {
            UserObj = new User();
            _databaseService = databaseService;
        }
        #endregion

        #region Commands
        [RelayCommand]
        public async Task Submit()
        {
            try
            {
                if (UserObj == null)
                {
                    await App.ShowAlert($"User is null!");
                    return;
                }
                if (string.IsNullOrEmpty(UserObj.Username) ||
                    string.IsNullOrEmpty(UserObj.Email) ||
                    string.IsNullOrEmpty(UserObj.FName) ||
                    string.IsNullOrEmpty(UserObj.LName) ||
                    string.IsNullOrEmpty(UserObj.Password))
                {
                    await App.ShowAlert($"Please make sure fields aren't empty!");
                    return;
                }

                // TODO: do validation on fields

                var response = await _databaseService.AddUserAsync(UserObj);
                if (response > 0)
                {
                    await App.ShowAlert($"Success! You can now login.");

                    UserObj = new User();
                    await Shell.Current.GoToAsync($"..", true);
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
        #endregion

    }
}
