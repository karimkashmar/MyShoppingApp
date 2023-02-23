using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyShoppingApp.Model;
using MyShoppingApp.Services;
using MyShoppingApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShoppingApp.ViewModel
{
    [QueryProperty("MyUser", "MyUser")]
    public partial class MyOrdersViewModel : BaseViewModel
    {
        [ObservableProperty]
        User myUser;
        public ObservableCollection<Order> UserOrders { get; set; }

        private DatabaseService _databaseService;

        public MyOrdersViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            UserOrders = new ObservableCollection<Order>();
            
        }
        [RelayCommand]
        async Task GoToDetails(Order order)
        {
            if (order == null)
                return;

            await Shell.Current.GoToAsync(nameof(OrderDetailsPage), true, new Dictionary<string, object>
        {
            {"Order", order }
        });
        }

        public async Task OnLoaded()
        {
            var userOrdersList = new List<Order>();
            userOrdersList = await _databaseService.GetOrdersByUserIDAsync(MyUser.UserID);
            foreach (var userOrder in userOrdersList)
            {
                var client = await _databaseService.GetClientByIDAsync(userOrder.ClientID);
                userOrder.Client = client;
            }

            if (userOrdersList != null && userOrdersList.Count > 0)
            {
                UserOrders.Clear();
            }

            foreach (var userOrder in userOrdersList)
            {
                UserOrders.Add(userOrder);
            }
        }
    }
}
