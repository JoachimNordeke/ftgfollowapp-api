using System.Collections.Generic;
using DocumentDb;

namespace API.Models.Sellers
{
    public class Seller : Document
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IEnumerable<string> StoreIds { get; set; }
    }
}
