using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Sales
{
    public class SaleSubscription
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Phonenumber { get; set; }
        public string StartDate { get; set; }
        public string RenewDate { get; set; }
        public string EndDate { get; set; }
    }
}
