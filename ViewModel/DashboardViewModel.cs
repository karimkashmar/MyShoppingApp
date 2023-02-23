using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyShoppingApp.Services;
using MyShoppingApp.View;
using MyShoppingApp.Model;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MyShoppingApp.ViewModel
{
    // DashboardViewModel inherits BaseViewModel to consequently inherit ObservableObject
    // DashboardViewModel is responsible for the logic in the page after login
    // QueryProperty allows the previous page to pass and assign the User object (currently logged in user)
    [QueryProperty("MyUser", "MyUser")]
    public partial class DashboardViewModel : BaseViewModel
    {

        public DashboardViewModel()
        {

        }

        internal Task OnLoaded()
        {
            return Task.CompletedTask;
        }

        [ObservableProperty]
        User myUser;


        [RelayCommand]
        public async Task Shop()
        {

            await Shell.Current.GoToAsync(nameof(ShoppingPage), true, new Dictionary<string, object>
                    {
                        {"MyUser", MyUser }
                    });
        }
        [RelayCommand]
        public async Task MyOrders()
        {

            await Shell.Current.GoToAsync(nameof(MyOrdersPage), true, new Dictionary<string, object>
                    {
                        {"MyUser", MyUser }
                    });
        }


    }
}
