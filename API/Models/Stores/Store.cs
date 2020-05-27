using DocumentDb;
using API.Models.Users;

namespace API.Models.Stores
{
    public class Store : Document
    {
        public string StoreId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string StoreManagerId { get; set; }
    }
}
