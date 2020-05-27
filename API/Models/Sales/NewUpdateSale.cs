using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Sales
{
    public class NewUpdateSale
    {
        public string Date { get; set; }
        public SaleSeller Seller { get; set; }
        public SaleCompany Company { get; set; }
        public IEnumerable<SaleExtraHardware> ChosenHardwares { get; set; }
        public IEnumerable<SaleExtraHardware> ChosenExtras { get; set; }
        public IEnumerable<SaleSubscription> ChosenSubscriptions { get; set; }
    }
}
