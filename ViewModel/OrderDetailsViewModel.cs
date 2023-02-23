using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyShoppingApp.Model;
using MyShoppingApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShoppingApp.ViewModel
{
    [QueryProperty("MyOrder", "Order")]
    public partial class OrderDetailsViewModel : BaseViewModel
    {
        [ObservableProperty]
        Order myOrder;
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

        private DatabaseService _databaseService;

        public ObservableCollection<OrderItem> OrderItems { get; set; }
        public ObservableCollection<Client> Clients { get; set; }


        public OrderDetailsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            OrderItems = new ObservableCollection<OrderItem>();
            Clients = new ObservableCollection<Client>();

            TotalAmount = 0;

            if (DateTime.Now.Hour < 12)
                MinDate = DateTime.Now;
            else
                MinDate = DateTime.Now.AddDays(1);
            MaxDate = DateTime.Today.AddMonths(60);
        }

        public async Task OnLoaded()
        {
            if (DateTime.Now.Hour < 12)
                MinDate = DateTime.Now;
            else
                MinDate = DateTime.Now.AddDays(1);
            MaxDate = DateTime.Today.AddMonths(60);

            var orderItemsList = new List<OrderItem>();
            orderItemsList = await _databaseService.GetOrderItemsByOrderIDAsync(MyOrder.OrderID);

            foreach (var orderItem in orderItemsList)
            {
                var item = await _databaseService.GetItemByIdAsync(orderItem.ItemID);
                orderItem.Item = item;
                orderItem.UpdatedRequestedAmount = orderItem.OrderQty;
            }

            var itemsFromDB = await _databaseService.GetItemsAsync();
            foreach (var item in itemsFromDB)
            {
                if (!orderItemsList.Select(oi => oi.ItemID).Contains(item.ItemID))
                {
                    orderItemsList.Add(new OrderItem()
                    {
                        ItemID = item.ItemID,
                        Item = item,
                        OrderID = MyOrder.OrderID,
                        OrderQty = 0,
                        UpdatedRequestedAmount = 0
                    });
                }
            }

            if (orderItemsList != null && orderItemsList.Count > 0)
            {
                OrderItems.Clear();
            }

            foreach (var orderItem in orderItemsList)
            {
                OrderItems.Add(orderItem);
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

            TotalAmount = MyOrder.TotalCost;
            Date = MyOrder.DateCreated;

            // Get the client object from the list used to populate the dropdown, not the client object coming with the order object
            // This will help populate the dropdown by default
            SelectedClient = Clients.SingleOrDefault(c => c.ClientID == MyOrder.Client.ClientID);

            // This does not work
            //SelectedClient = MyOrder.Client;
        }


        [RelayCommand]
        public async Task SaveOrder()
        {
            bool isItemRequested = false;

            if (SelectedClient == null || SelectedClient.ClientID < 1)
            {
                await App.ShowAlert("Please select a client from the drop down menu");
                return;
            }

            foreach (var orderItem in OrderItems)
            {
                if (orderItem.UpdatedRequestedAmount > 0)
                {
                    isItemRequested = true;
                }
                if (orderItem.UpdatedRequestedAmount > (orderItem.Item.QtyInStock + orderItem.OrderQty))
                {
                    await App.ShowAlert($"You requested {orderItem.UpdatedRequestedAmount} of {orderItem.Item.Name}, but we only have {orderItem.Item.QtyInStock} in stock in addition to the previously ordered quantity.");
                    return;
                }
            }
            if (!isItemRequested)
            {
                await App.ShowAlert("Please select at least 1 item, or delete the order.");
                return;
            }

            bool allUpdatedCorrectly = true;
            foreach (var orderItem in OrderItems)
            {
                if (orderItem.OrderQty == 0 && orderItem.UpdatedRequestedAmount > 0)
                {
                    // If item was not previously ordered, create new OrderItem

                    var isOrderItemRequestSuccess = await _databaseService.CreateOrderItemAsync(new OrderItem()
                    {
                        ItemID = orderItem.ItemID,
                        OrderID = MyOrder.OrderID,
                        OrderQty = orderItem.UpdatedRequestedAmount
                    });
                    if (isOrderItemRequestSuccess)
                    {
                        orderItem.Item.QtyInStock -= orderItem.UpdatedRequestedAmount;
                        var isItemUpdateRequestSuccess = await _databaseService.UpdateItemAsync(orderItem.Item);
                        if (!isItemUpdateRequestSuccess)
                        {
                            await App.ShowAlert($"Failed to update item qty for {orderItem.Item.Name}");
                            allUpdatedCorrectly = false;
                        }
                    }
                }
                else
                {
                    if (orderItem.OrderQty != 0 && orderItem.UpdatedRequestedAmount == 0)
                    {
                        // If item was already in order, and is updated to 0, remove it


                        var response = await _databaseService.DeleteOrderItemAsync(orderItem.OrderItemID);
                        if (!response)
                        {
                            allUpdatedCorrectly = false;
                        }
                        else
                        {
                            orderItem.Item.QtyInStock += orderItem.OrderQty;
                            var isItemUpdateRequestSuccess = await _databaseService.UpdateItemAsync(orderItem.Item);
                            if (!isItemUpdateRequestSuccess)
                            {
                                await App.ShowAlert($"Failed to update item qty for {orderItem.Item.Name}");
                                allUpdatedCorrectly = false;
                            }
                        }
                    }
                    else if (orderItem.OrderQty != 0)
                    {
                        // If item was already in order, and is updated to > 0, update it
                        var previousOrderQty = orderItem.OrderQty;
                        orderItem.OrderQty = orderItem.UpdatedRequestedAmount;
                        var response = await _databaseService.UpdateOrderItemAsync(orderItem);

                        if (!response)
                        {
                            allUpdatedCorrectly = false;
                        }
                        else
                        {
                            orderItem.Item.QtyInStock += previousOrderQty - orderItem.UpdatedRequestedAmount;
                            var isItemUpdateRequestSuccess = await _databaseService.UpdateItemAsync(orderItem.Item);
                            if (!isItemUpdateRequestSuccess)
                            {
                                await App.ShowAlert($"Failed to update item qty for {orderItem.Item.Name}");
                                allUpdatedCorrectly = false;
                            }
                        }
                    }
                }
            }

            if (allUpdatedCorrectly)
            {
                MyOrder.ClientID = SelectedClient.ClientID;
                MyOrder.TotalCost = TotalAmount;
                MyOrder.DateCreated = Date;
                var response = await _databaseService.UpdateOrderAsync(MyOrder);

                if (response)
                {
                    await App.ShowAlert($"Order updated successfully!");
                    await Shell.Current.GoToAsync("..", true);
                }
                else
                {
                    await App.ShowAlert($"Failed to update order");
                }
            }
            else
            {
                await App.ShowAlert($"Failed to update order items");
            }




        }

        [RelayCommand]
        public async Task DeleteOrder()
        {
            bool deleteConfirmation = await Shell.Current.DisplayAlert("Deleting Order", "Are you sure you want to delete this order?", "Yes", "No");
            if (deleteConfirmation)
            {
                bool allDeletedSuccessfully = true;
                foreach (var orderItem in OrderItems)
                {
                    var response = await _databaseService.DeleteOrderItemAsync(orderItem.OrderItemID);

                    if (!response)
                    {
                        allDeletedSuccessfully = false;
                    }
                }

                if (allDeletedSuccessfully)
                {
                    var orderDeleteResponse = await _databaseService.DeleteOrderAsync(MyOrder.OrderID);

                    if (orderDeleteResponse)
                    {
                        await App.ShowAlert($"Your order was deleted succesfully!");
                        await Shell.Current.GoToAsync("..", true);
                    }
                    else
                    {
                        await App.ShowAlert($"Your order failed to delete");
                    }
                }
            }
        }


        public async Task OnRequestedAmountChanged()
        {
            var cost = 0.0;

            foreach (var orderItem in OrderItems)
            {
                cost += Convert.ToDouble(orderItem.UpdatedRequestedAmount) * orderItem.Item.Price;
            }

            TotalAmount = cost;
        }
    }
}
