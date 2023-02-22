﻿using CommunityToolkit.Mvvm.ComponentModel;
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

                var  foundUser = await _databaseService.ValidatePasswordAsync(Username, Password);
                if (foundUser == null)
                {
                    await App.ShowAlert($"Error: username or password invalid.");
                    return;
                }
                else
                {
                    await Shell.Current.GoToAsync(nameof(DashboardPage), true);
                }
            }
            catch (Exception ex)
            {
                await App.ShowAlert($"Error: {ex.Message}");
            }
        }
    }
}
