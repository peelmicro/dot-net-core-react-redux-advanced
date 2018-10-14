using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreReactReduxAdvanced.Models;
using NetCoreReactReduxAdvanced.Services;
namespace NetCoreReactReduxAdvanced.Controllers
{
  public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(IUserService userService, SignInManager<ApplicationUser> signInManager)
        {
            _userService = userService;
            _signInManager = signInManager;
        }

//        [HttpGet]
//        public IActionResult Index()
//        {
//            return Ok("");
//        }

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

            var user = await _userService.CreateUserFromEmail(emailAddress);

            await _userService.AddLoginIfDoesNotExist(user, info);

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
            var user = await _userService.GetUser(HttpContext.User);
            return user;
        }
    }
}