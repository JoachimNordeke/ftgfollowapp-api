using API.Models.Sales;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Factories
{
    public static class SaleFactory
    {
        public static Sale CreateSale(string storeId, int commission, NewUpdateSale sale)
        {
            return new Sale
            {
                Id = Guid.NewGuid().ToString(),
                SellerId = sale.Seller.Id,
                CompanyId = sale.Company.Id,
                SaleDate = DateTime.Parse(sale.Date),
                Hardwares = sale.ChosenHardwares,
                Extras = sale.ChosenExtras,
                Subscriptions = ExtractSubscriptions(sale.ChosenSubscriptions),
                StoreId = storeId,
                Commission = commission
            };
        }

        private static IEnumerable<Subscription> ExtractSubscriptions(IEnumerable<SaleSubscription> subscriptions)
        {
            return subscriptions.Select(x => new Subscription
            {
                Name = x.Title,
                PhoneNumber = x.Phonenumber,
                StartDate = DateTime.Parse(x.StartDate),
                RenewDate = DateTime.Parse(x.RenewDate),
                EndDate = DateTime.Parse(x.EndDate)
            });
        }
    }
}
