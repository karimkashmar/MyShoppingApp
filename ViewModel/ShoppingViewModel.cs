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
            return;
        }
    }
}
