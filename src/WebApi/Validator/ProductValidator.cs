using Products;
using Products.Models;
using Products.ProductManager;

namespace WebApi.Validator;

public class ProductValidator
{
    private static readonly ProductsManager Products;

    public bool Validate(Product? product)
    {
        if (product != null && product.Name != string.Empty && product.Price > 0)
        {
            return true;
        }

        return false;
    }

    public bool ProductExists(Product product)
    {
        return Products.GetProducts().Any(e => e.ID == product.ID);
    }
}