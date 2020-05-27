using DocumentDb;

namespace API.Models.Products
{
    public class Product : Document
    {
        public string Title { get; set; }
        public int Commission { get; set; }
        public string Type { get; set; }
    }
}
