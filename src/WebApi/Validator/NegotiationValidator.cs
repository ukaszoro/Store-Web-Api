using Negotiations.Models;
using Negotiations.NegotiationManager;
using Products.ProductManager;

namespace WebApi.Validator;

public class NegotiationValidator(INegotiationManager negotiationManager, IProductsManager productsManager)
{
    private readonly ProductValidator _productValidator = new ProductValidator(productsManager);

    public bool Validate(INegotiationBase? negotiation)
    {
        if (negotiation != null && negotiation.price > 0 && _productValidator.Exists(negotiation.ProductId))
        {
            return true;
        }

        return false;
    }

    public bool Exists(string userId, long productId)
    {
        return negotiationManager.GetAll().Any(e => e.UserId == userId && e.ProductId == productId);
    }
}