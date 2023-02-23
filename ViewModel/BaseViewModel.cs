using CommunityToolkit.Mvvm.ComponentModel;

namespace MyShoppingApp.ViewModel
{


    // BaseViewModel inherits ObservableObject from CommunityToolkit to handle INotifyProperty automatically
    
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        string title;
    
    }

}
