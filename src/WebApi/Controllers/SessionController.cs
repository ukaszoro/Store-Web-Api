using Microsoft.AspNetCore.Mvc;
using WebApi.SessionManager;

namespace WebApi.Controllers;

[Route("api/Session")]
[ApiController]
public class SessionController(IEmployeeSessionManager employeeSessionManager) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult Login()
    {
        Response.Cookies.Append("session", employeeSessionManager.NewSession(),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddDays(30) });
        return Ok();
    }

    [HttpPost("check")]
    public ActionResult Check()
    {
        if (employeeSessionManager.Exists(Request.Cookies["session"] ?? String.Empty))
        {
            return Ok("Logged in");
        }
        else
        {
            return Unauthorized();
        }
    }
}