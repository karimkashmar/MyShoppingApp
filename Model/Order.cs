namespace MyShoppingApp.Model
{
    public class Order
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public string DeliveryAddress { get; set; }
        public double TotalCost { get; set; }
    }
}