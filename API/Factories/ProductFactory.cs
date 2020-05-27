using System;
using API.Models.Products;

namespace API.Factories
{
    public static class ProductFactory
    {
        public static Product CreateProduct(NewUpdateProduct newProduct)
        {
            return new Product
            {
                Id = Guid.NewGuid().ToString(),
                Title = newProduct.Title,
                Commission = newProduct.Commission,
                Type = newProduct.Type
            };
        }

        public static Product UpdateProduct(Product product, NewUpdateProduct update)
        {
            product.Title = update.Title;
            product.Commission = update.Commission;
            product.Type = update.Type;

            return product;
        }
    }
}
