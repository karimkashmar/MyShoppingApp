namespace MyShoppingApp.Model
{
    public class Item
    {
        public int ID { get; set; }
        public double Price { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string SDescription { get; set; }
        public string LDescription { get; set; }
        public int QtyInStock { get; set; }
        public int TrendingRating { get; set; }
    }
}
