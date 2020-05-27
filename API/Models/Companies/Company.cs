using DocumentDb;

namespace API.Models.Companies
{
    public class Company : Document
    {
        public string StoreId { get; set; }
        public string Name { get; set; }
        public string OrgNumber { get; set; }
        public string StreetAddress { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
    }
}
