using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models.Sales
{
    public class Subscription
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime RenewDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
