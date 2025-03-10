using Products;
using Products.Models;
using Products.ProductManager;
using WebApi.DTO;

namespace WebApi.Validator;

public class ProductValidator(IProductsManager productsManager)
{
    public bool Validate(ProductDTO product)
    {
        if (product.Name != string.Empty && product.Price > 0)
        {
            return true;
        }

        return false;
    }

    public bool Exists(long productId)
    {
        return productsManager.GetAll().Any(e => e.Id == productId);
    }
}