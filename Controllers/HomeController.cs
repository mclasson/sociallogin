using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

using Microsoft.Extensions.DependencyInjection;

namespace customlogin.Controllers
{
            [Authorize]
    public class HomeController : Controller
    {
        readonly SignInManager<ClaimsIdentity> _signInManager = null;
        public HomeController()
        {
         
        }
        public async Task<IActionResult> Index()
        {
            
            
            var principal = User.Identity as ClaimsIdentity;
            var idClaim = principal.Claims.Where(i => i.Type == "http://marcusclasson.com/claims/id").SingleOrDefault();
            if(idClaim == null)
            {
                
                principal.AddClaim(new Claim("http://marcusclasson.com/claims/id", "MyCustomId"));
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(User);
            }
            return View();
        }

 
        public async Task<IActionResult> Logout()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index");
            var user = HttpContext.User.Identity.Name;

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (username == "kalle")
            {
                        var claims = new List<Claim>(2);
        claims.Add(new Claim(ClaimTypes.Name, username));

        claims.Add(new Claim(ClaimTypes.Role, "GroupThatUserIsIn", 
            ClaimValueTypes.String, "IHaveIssuedThis"));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
                return RedirectToAction("Index");
            }
            return View();
        }

    }
}
