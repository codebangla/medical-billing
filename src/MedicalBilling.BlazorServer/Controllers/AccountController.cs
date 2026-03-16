using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace MedicalBilling.BlazorServer.Controllers;

[Route("[controller]/[action]")]
public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string redirectUri = "/")
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = redirectUri
        }, "oidc");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        await HttpContext.SignOutAsync("oidc");
        return Redirect("/");
    }
}
