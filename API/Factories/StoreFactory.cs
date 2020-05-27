using System;
using API.Models.Stores;

namespace API.Factories
{
    public static class StoreFactory
    {
        public static Store CreateStore(NewUpdateStore newStore)
        {
            return new Store
            {
                Id = Guid.NewGuid().ToString(),
                StoreId = newStore.StoreId,
                Name = newStore.Name,
                Address = newStore.Address,
                StoreManagerId = newStore.StoreManagerId
            };
        }

        public static Store UpdateStore(Store store, NewUpdateStore updateStore)
        {
            store.StoreId = updateStore.StoreId;
            store.Name = updateStore.Name;
            store.Address = updateStore.Address;
            store.StoreManagerId = updateStore.StoreManagerId;

            return store;
        }
    }
}
