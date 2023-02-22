using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyShoppingApp.Model;
using MyShoppingApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShoppingApp.ViewModel
{
    public partial class ShoppingViewModel : BaseViewModel
    {
        private DatabaseService _databaseService;

        public ObservableCollection<Item> Items { get; set; }
        [ObservableProperty]
        double totalAmount;

        public ShoppingViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Items = new ObservableCollection<Item>();
            totalAmount = 0;
        }

        [RelayCommand]
        public async Task PlaceOrder()
        {
            var x = Items;
        }

        public async Task OnLoaded()
        {
            var list = new List<Item>();
            list = await _databaseService.GetItemsAsync();

            if (list != null && list.Count > 0)
            {
                Items.Clear();
            }

            foreach (var item in list)
            {
                Items.Add(item);
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
