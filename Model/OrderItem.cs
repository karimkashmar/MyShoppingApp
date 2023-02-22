namespace MyShoppingApp.Model
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ItemID { get; set; }
        public int OrderID { get; set; }
        public int OrderQty { get; set; }
    }
}
