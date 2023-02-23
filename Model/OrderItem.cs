namespace MyShoppingApp.Model
{
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        public int ItemID { get; set; }
        public int OrderID { get; set; }
        public int OrderQty { get; set; }
    }
}
