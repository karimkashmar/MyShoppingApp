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

    [QueryProperty("MyUser","MyUser")]
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
            
           await Shell.Current.GoToAsync(nameof(ShoppingPage), true);

        }
    }
}
