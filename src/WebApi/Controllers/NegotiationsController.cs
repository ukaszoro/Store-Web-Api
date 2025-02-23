using Microsoft.AspNetCore.Mvc;
using Negotiations;
using Negotiations.Models;
using Negotiations.NegotiationManager;
using Products.ProductManager;
using WebApi.DTO;
using WebApi.SessionManager;
using WebApi.Validator;

namespace WebApi.Controllers;

[Route("api/Products")]
[ApiController]
public class NegotiationsController(
    INegotiationManager negotiationManager,
    IProductsManager productsManager,
    ISessionManager sessionManager,
    IUserSessionManager userSessionManager) : ControllerBase
{
    private NegotiationValidator _negotiationValidator = new NegotiationValidator(negotiationManager, productsManager);

    [HttpGet("{productId}/bids")]
    public async Task<ActionResult<IEnumerable<Negotiation>>> GetNegotiations(long productId)
    {
        return await negotiationManager.GetAllWithProductId(productId);
    }

    [HttpPost("{productId}/bids")]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> CreateBid(long productId, NegotiationDto negotiationDto)
    {
        var cookie = Request.Cookies["client"];
        if (!userSessionManager.Exists(cookie ?? String.Empty))
        {
            cookie = userSessionManager.NewSession();
            Response.Cookies.Append("client", cookie,
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) });
        }

        if (!_negotiationValidator.Validate(negotiationDto) || negotiationDto.ProductId != productId)
        {
            return BadRequest();
        }

        Negotiation? negotiation;

        if (_negotiationValidator.Exists(cookie, negotiationDto.ProductId))
        {
            negotiation = await negotiationManager.Find(cookie, negotiationDto.ProductId);
            if (negotiation.status == NegotiationStatus.rejected)
            {
                negotiation.status = NegotiationStatus.waiting;
                negotiation.TimesRejected += 1;
                negotiation.RejectedAt = null;
            }
            else
            {
                return BadRequest();
            }

            await negotiationManager.SaveChanges();
        }
        else
        {
            negotiation = new Negotiation
            {
                ProductId = negotiationDto.ProductId,
                price = negotiationDto.price,
                UserId = cookie,
                TimesRejected = 0,
                status = NegotiationStatus.waiting
            };
            await negotiationManager.Add(negotiation);
        }

        return Ok();
    }

    private static NegotiationDto ItemToDto(Negotiation negotiation) =>
        new NegotiationDto()
        {
            ProductId = negotiation.ProductId,
            price = negotiation.price,
        };
}