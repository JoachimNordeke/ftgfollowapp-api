using DocumentDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Sales
{
    public class Sale : Document
    {
        public string StoreId { get; set; }
        public DateTime SaleDate { get; set; }
        public string SellerId { get; set; }
        public string CompanyId { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
        public IEnumerable<SaleExtraHardware> Hardwares { get; set; }
        public IEnumerable<SaleExtraHardware> Extras { get; set; }
        public int Commission { get; set; }
    }
}
