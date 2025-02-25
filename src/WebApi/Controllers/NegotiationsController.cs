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
    IEmployeeSessionManager employeeSessionManager,
    IUserSessionManager userSessionManager) : ControllerBase
{
    private readonly NegotiationValidator _negotiationValidator =
        new NegotiationValidator(negotiationManager, productsManager);

    [HttpGet("{productId}/bids")]
    public async Task<ActionResult<IEnumerable<Negotiation>>> GetNegotiations(long productId)
    {
        if (!employeeSessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        return await negotiationManager.GetAllWithProductId(productId);
    }

    [HttpGet("{productId}/bids/{negotiationId}")]
    public async Task<ActionResult<NegotiationDto>> GetNegotiation(long productId, long negotiationId)
    {
        if (!_negotiationValidator.Exists(negotiationId))
        {
            return NotFound();
        }

        var negotiation = (await negotiationManager.Find(negotiationId));
        if (negotiation is null)
        {
            return NotFound();
        }

        return new NegotiationDto()
        {
            Id = negotiation.Id,
            ProductId = negotiation.ProductId,
            Price = negotiation.Price,
            Status = negotiation.Status,
        };
    }

    [HttpPost("{productId}/bids")]
    public async Task<ActionResult> CreateBid(long productId, NegotiationDto negotiationDto)
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

        if (cookie is null)
        {
            return Unauthorized();
        }

        Negotiation? negotiation;

        if (_negotiationValidator.Exists(cookie, negotiationDto.ProductId))
        {
            negotiation = await negotiationManager.Find(cookie, negotiationDto.ProductId);
            if (negotiation is null)
            {
                return NotFound();
            }

            if (negotiation.Status == NegotiationStatus.rejected)
            {
                negotiation.Status = NegotiationStatus.waiting;
                negotiation.RejectedAt = null;
                negotiation.Price = negotiationDto.Price;
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
                Price = negotiationDto.Price,
                UserId = cookie,
                TimesRejected = 0,
                Status = NegotiationStatus.waiting
            };
            await negotiationManager.Add(negotiation);
        }

        return Ok();
    }

    [HttpDelete("{productId}/bids/{negotiationId}")]
    public async Task<IActionResult> DeleteNegotiation(long productId, long negotiationId)
    {
        if (!employeeSessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        Negotiation? negotiation = await negotiationManager.Find(negotiationId);
        if (negotiation is null)
        {
            return NotFound();
        }
        else if (productId != negotiation.ProductId)
        {
            return BadRequest();
        }

        await negotiationManager.Remove(negotiation);
        await negotiationManager.SaveChanges();

        return NoContent();
    }

    [HttpPost("{productId}/bids/{negotiationId}")]
    public async Task<ActionResult> CreateBid(long productId, long negotiationId,
        NegotiationDto negotiationNew)
    {
        if (!employeeSessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Unauthorized();
        }

        else if (productId != negotiationNew.ProductId || negotiationNew.Id != negotiationId)
        {
            return BadRequest();
        }

        Negotiation? negotiation = await negotiationManager.Find(negotiationId);
        if (negotiation is null)
        {
            return NotFound();
        }

        if (negotiationNew.Status == NegotiationStatus.rejected)
        {
            negotiation.Status = NegotiationStatus.rejected;
            negotiation.TimesRejected += 1;
            negotiation.RejectedAt = DateTime.UtcNow;
            await negotiationManager.SaveChanges();
        }
        else if (negotiationNew.Status == NegotiationStatus.accepted)
        {
            negotiation.Status = NegotiationStatus.accepted;
        }
        else
        {
            return BadRequest();
        }

        if (negotiation.TimesRejected > 3)
        {
            negotiation.Status = NegotiationStatus.closed;
        }

        return Ok();
    }
}