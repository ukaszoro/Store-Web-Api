using Microsoft.AspNetCore.Mvc;
using Negotiations;
using Negotiations.Models;
using Negotiations.NegotiationManager;
using Products.Models;
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
        if (!sessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        return await negotiationManager.GetAllWithProductId(productId);
    }

    [HttpGet("{productid}/bids/{negotiationid}")]
    public async Task<ActionResult<NegotiationDto>> GetNegotiation(long productId, long negotiationid)
    {
        if (!_negotiationValidator.Exists(negotiationid))
        {
            return NotFound();
        }

        return ItemToDto((await negotiationManager.Find(productId))!);
    }

    [HttpPost("{productId}/bids")]
    public async Task<ActionResult<IEnumerable<Product>>> CreateBid(long productId, NegotiationDto negotiationDto)
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
                negotiation.RejectedAt = null;
                negotiation.price = negotiationDto.price;
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

    [HttpDelete("{productid}/bids/{negotiationid}")]
    public async Task<IActionResult> DeleteNegotiation(long productid, long negotiationid)
    {
        if (!sessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        Negotiation? negotiation = await negotiationManager.Find(negotiationid);
        if (negotiation is null)
        {
            return NotFound();
        }
        else if (productid != negotiation.ProductId)
        {
            return BadRequest();
        }

        await negotiationManager.Remove(negotiation);
        await negotiationManager.SaveChanges();

        return NoContent();
    }

    [HttpPost("{productid}/bids/{negotiationid}")]
    public async Task<ActionResult<IEnumerable<Product>>> CreateBid(long productid, long negotiationid,
        Negotiation? negotiationNew)
    {
        if (!sessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        if (negotiationNew is null)
        {
            return NotFound();
        }
        else if (productid != negotiationNew.ProductId || negotiationNew.Id != negotiationid)
        {
            return BadRequest();
        }

        Negotiation? negotiation = await negotiationManager.Find(negotiationid);
        if (negotiation is null)
        {
            return NotFound();
        }

        if (negotiationNew.status == NegotiationStatus.rejected)
        {
            negotiation.status = NegotiationStatus.rejected;
            negotiation.TimesRejected += 1;
            negotiation.RejectedAt = DateTime.Now;
            await negotiationManager.SaveChanges();
        }
        else if (negotiationNew.status == NegotiationStatus.accepted)
        {
            negotiation.status = NegotiationStatus.accepted;
        }

        if (negotiation.TimesRejected > 3)
        {
            negotiation.status = NegotiationStatus.closed;
        }

        return Ok();
    }

    private static NegotiationDto ItemToDto(Negotiation negotiation) =>
        new NegotiationDto()
        {
            ProductId = negotiation.ProductId,
            price = negotiation.price,
            status = negotiation.status,
        };
}