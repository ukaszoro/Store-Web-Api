using Negotiations;
using Negotiations.Models;
using Negotiations.NegotiationManager;
using Products.ProductManager;
using WebApi.DTO;

namespace WebApi.Validator;

public class NegotiationValidator(INegotiationManager negotiationManager, IProductsManager productsManager)
{
    private readonly ProductValidator _productValidator = new ProductValidator(productsManager);

    public bool Validate(NegotiationDto negotiation)
    {
        if (negotiation.Price > 0 && _productValidator.Exists(negotiation.ProductId))
        {
            return true;
        }

        return false;
    }

    public bool Exists(string userId, long productId)
    {
        return negotiationManager.GetAll().Any(e => e.UserId == userId && e.ProductId == productId);
    }

    public bool Exists(long negotiationId)
    {
        return negotiationManager.GetAll().Any(e => e.Id == negotiationId);
    }
}