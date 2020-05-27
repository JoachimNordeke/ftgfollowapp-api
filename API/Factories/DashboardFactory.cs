using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Factories
{
    public class DashboardFactory
    {

        public object Dashboard()
        {
            return new {
                NiceCardData = GetNiceCardData(),
                HighestCommission = GetHighestCommission(),
                ExpireSoonSubscriptions = GetExpireSoonSubscriptions(),
                ExpiredSubscriptions = GetExpiredSubscriptions(),
                TotalOrderValues = GetTotalOrderValues(),
                PieChartValues = GetPieChartValues(),
                BudgetData = GetBudgetData()
            };
        }

        private object[] GetNiceCardData()
        {
            return new object[] { 
                new { title = "Företagsabonnemang (Månad)", value = 23 },
                new { title = "Företagsabonnemang (Kvartal)", value = 38 },
                new { title = "Högsta provision (Mimmi)", value = 367 },
                new { title = "En fjärde grej", value = 1234 }
            };
        }

        private object GetHighestCommission()
        {
            return new { Seller = "Mimmi", Company = "Nisses Städ AB", Date = "2019-10-02", Commission = 367 };
        }

        private object[] GetExpireSoonSubscriptions()
        {
            return new [] { 
                new { Company = "Hejkomohjälpmig AB", Subscription = "FAST 10gb 24mån", EndDate = "2020-01-02" },
                new { Company = "Coola killar AB", Subscription = "UNLIMITED 24mån", EndDate = "2020-01-13" }
            };
        }

        private List<object> GetExpiredSubscriptions()
        {
            var list = new List<object> {
                new { Company = "Alleballe AB", Subscription = "Rörligt 5gb 12mån", EndDate = "2019-11-02" }
            };

            return list;
        }

        private object[][] GetTotalOrderValues()
        {
            return new object[][] { 
                new [] { "Jan", "Feb", "Mar", "Apr", "Maj", "Jun", "Jul", "Aug", "Sep" },
                new object[] { 4763, 6541, 1872, 4678, 10000, 20000, 15000, 25000, 35000 }
            };
        }

        private object[][] GetPieChartValues()
        {
            return new object[][] { 
                new [] { "Data 1", "Data 2", "Data 3"},
                new object[] { 111, 222, 333 }
            };
        }

        private object[] GetBudgetData()
        {
            return new[] { 
                new { Seller = "Mimmi", Actual = 23, Budget = 15 },
                new { Seller = "Fredrik", Actual = 7, Budget = 12 },
                new { Seller = "Nathalie", Actual = 3, Budget = 10 },
                new { Seller = "Adam", Actual = 2, Budget = 12 }
            };
        }
    }
}
