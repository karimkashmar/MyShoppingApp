using CommunityToolkit.Mvvm.ComponentModel;
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
    public class ShoppingViewModel : BaseViewModel
    {
        private DatabaseService _databaseService;

        public ObservableCollection<Item> Items { get; set; }

        public ShoppingViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            Items = new ObservableCollection<Item>();
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
    }
}
