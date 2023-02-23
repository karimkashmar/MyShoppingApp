using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyShoppingApp.Model;
using MyShoppingApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace MyShoppingApp.ViewModel
{
    public partial class ShoppingViewModel : BaseViewModel
    {
        private DatabaseService _databaseService;

        public ObservableCollection<Item> Items { get; set; }
        public ObservableCollection<Client> Clients { get; set; }


        [ObservableProperty]
        double totalAmount;

        [ObservableProperty]
        Client selectedClient;

        [ObservableProperty]
        DateTime date;
        [ObservableProperty]
        DateTime minDate;
        [ObservableProperty]
        DateTime maxDate;

        public ShoppingViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Items = new ObservableCollection<Item>();
            Clients = new ObservableCollection<Client>();
            SelectedClient = new Client();
            totalAmount = 0;


            if (DateTime.Now.Hour < 12)
                minDate = DateTime.Now;
            else
                minDate = DateTime.Now.AddDays(1);
            maxDate = DateTime.Today.AddMonths(60);
        }

        [RelayCommand]
        public async Task PlaceOrder()
        {
            int clientID = SelectedClient.ClientID;
        }

        public async Task OnLoaded()
        {
            var itemsList = new List<Item>();
            itemsList = await _databaseService.GetItemsAsync();

            if (itemsList != null && itemsList.Count > 0)
            {
                Items.Clear();
            }

            foreach (var item in itemsList)
            {
                Items.Add(item);
            }

            var clientsList = new List<Client>();
            clientsList = await _databaseService.GetClientsAsync();
            if (clientsList != null && clientsList.Count > 0)
            {
                Clients.Clear();
            }

            foreach (var client in clientsList)
            {
                Clients.Add(client);
            }




        }

        public async Task OnRequestedAmountChanged()
        {
            var cost = 0.0;

            foreach (var item in Items)
            {
                if (item.RequestedAmount > item.QtyInStock)
                {
                    item.RequestedAmount = item.QtyInStock;

                    OnPropertyChanged(nameof(Items));
                }

                cost += Convert.ToDouble(item.RequestedAmount) * item.Price;
            }

            TotalAmount = cost;
        }
    }
}
