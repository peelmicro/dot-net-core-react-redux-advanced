using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreReactReduxAdvanced.Models;

namespace NetCoreReactReduxAdvanced.Controllers
{
  public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            return Ok("");
        }

        [HttpGet]
        public IActionResult Google(string returnUrl = null)
        {
            return ExternalLogin("Google", returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Auth", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl = null, string remoteError = null)
        {
            if (!string.IsNullOrEmpty(remoteError))
            {
                return BadRequest();
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest();
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true, true);

            if (result.Succeeded)
            {
                return Redirect("/Blogs");
            }

            var emailAddress = info.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new ApplicationUser
            {
                Email = emailAddress,
                UserName = emailAddress,
                SecurityStamp = new Guid().ToString()
            };
            var identityUser = await _userManager.FindByEmailAsync(emailAddress);
            if (identityUser == null)
            {
                await _userManager.CreateAsync(user);
            }

            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null ||
                !logins.Any(x => x.LoginProvider == info.LoginProvider && x.ProviderKey == info.ProviderKey))
            {
                await _userManager.AddLoginAsync(user, info);
            }

            await _signInManager.SignInAsync(user, true);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("/Blogs");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Blogs");
        }

        [HttpGet]
        [Route("Api/Current_User")]
        [Route("Auth/Current_User")]
        public async Task<ApplicationUser> CurrentUser()
        {
            
            var user = HttpContext.User == null ? null : await _userManager.GetUserAsync(HttpContext.User);
            return user;
        }
    }
}