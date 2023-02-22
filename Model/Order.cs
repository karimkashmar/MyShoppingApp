namespace MyShoppingApp.Model
{
    public class Order
    {
        public int OrderID { get; set; }
        public User User { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public string DeliveryAddress { get; set; }
        public double TotalCost { get; set; }
    }
}