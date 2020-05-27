using System;
using API.Models.Sellers;

namespace API.Factories
{
    public static class SellerFactory
    {
        public static Seller CreateSeller(NewUpdateSeller newSeller)
        {
            return new Seller
            {
                Id = Guid.NewGuid().ToString(),
                Firstname = newSeller.Firstname,
                Lastname = newSeller.Lastname,
                Email = newSeller.Email,
                Phone = newSeller.Phone,
                StoreIds = newSeller.StoreIds
            };
        }

        public static Seller UpdateSeller(Seller seller, NewUpdateSeller updateSeller)
        {
            seller.Firstname = updateSeller.Firstname;
            seller.Lastname = updateSeller.Lastname;
            seller.Email = updateSeller.Email;
            seller.Phone = updateSeller.Phone;
            seller.StoreIds = updateSeller.StoreIds;

            return seller;
        }
    }
}
