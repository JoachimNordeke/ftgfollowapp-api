using System.Collections.Generic;

namespace API.Models.Sellers
{
    public class NewUpdateSeller
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public IEnumerable<string> StoreIds { get; set; }
    }
}
