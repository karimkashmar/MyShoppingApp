using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyShoppingApp.Model;
using MyShoppingApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace MyShoppingApp.ViewModel
{
    [QueryProperty("MyUser", "MyUser")]

    public partial class ShoppingViewModel : BaseViewModel
    {
        private DatabaseService _databaseService;

        public ObservableCollection<Item> Items { get; set; }
        public ObservableCollection<Client> Clients { get; set; }


        [ObservableProperty]
        User myUser;
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
                MinDate = DateTime.Now;
            else
                MinDate = DateTime.Now.AddDays(1);
            MaxDate = DateTime.Today.AddMonths(60);

        }

        [RelayCommand]
        public async Task PlaceOrder()
        {
            bool isItemRequested = false;

            if (SelectedClient == null || SelectedClient.ClientID<1)
            {
                await App.ShowAlert("Please select a client from the drop down menu");
                return;
            }


            foreach (var item in Items)
            {
                if (item.RequestedAmount > 0)
                {
                    isItemRequested = true;
                }
                if (item.RequestedAmount > item.QtyInStock)
                {
                    await App.ShowAlert($"You requested {item.RequestedAmount} of {item.Name}, but we only have {item.QtyInStock} in stock.");
                    return;
                }
            }
            if (!isItemRequested)
            {
                await App.ShowAlert("Please select at least 1 item.");
                return;
            }

            int orderId = await _databaseService.CreateOrderAsync(new Order()
            {
                ClientID = SelectedClient.ClientID,
                DateCreated = Date,
                TotalCost = TotalAmount,
                UserID = MyUser.UserID
            });
            bool isOrderSuccessful = true;

            if (orderId < 1)
            {
                await App.ShowAlert("Failed to add order, please try again.");
                isOrderSuccessful = false;

            }
            else
            {
                foreach (var item in Items)
                {
                    if (item.RequestedAmount > 0)
                    {
                        var isOrderItemRequestSuccess = await _databaseService.CreateOrderItemAsync(new OrderItem()
                        {
                            ItemID = item.ItemID,
                            OrderID = orderId,
                            OrderQty = item.RequestedAmount
                        });
                        if (isOrderItemRequestSuccess)
                        {
                            item.QtyInStock -= item.RequestedAmount;
                            var isItemUpdateRequestSuccess = await _databaseService.UpdateItemAsync(item);
                            if (!isItemUpdateRequestSuccess)
                            {
                                await App.ShowAlert($"Failed to update item qty for {item.Name}");
                                isOrderSuccessful = false;
                            }
                        }
                        else { isOrderSuccessful = false; }
                    }
                }
            }
            if (isOrderSuccessful)
            {
                await App.ShowAlert($"Success! {SelectedClient.EmailAddress} will be contacted on {Date.ToString("dd-MMM-yyyy")}");
                Items.Clear();
                Clients.Clear();
                SelectedClient = null;
                await Shell.Current.GoToAsync("..", true, new Dictionary<string, object>
                    {
                        {"MyUser", MyUser }
                    });
            }
        }

        public async Task OnLoaded()
        {
            if (DateTime.Now.Hour < 12)
                MinDate = DateTime.Now;
            else
                MinDate = DateTime.Now.AddDays(1);
            MaxDate = DateTime.Today.AddMonths(60);

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
                cost += Convert.ToDouble(item.RequestedAmount) * item.Price;
            }

            TotalAmount = cost;
        }
    }
}
