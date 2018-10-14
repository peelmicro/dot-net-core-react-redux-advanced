using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NetCoreReactReduxAdvanced.Models;

namespace NetCoreReactReduxAdvanced.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> CreateUserFromEmail(string emailAddress = null)
        {
            var securityStamp =  Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(emailAddress))
            {
                emailAddress = $"{securityStamp.Replace("-","")}@mail.com";
            }
            var user = new ApplicationUser
            {
                Email = emailAddress,
                UserName = emailAddress,
                SecurityStamp = securityStamp
            };
            var identityUser = await _userManager.FindByEmailAsync(emailAddress);
            if (identityUser == null)
            {
                await _userManager.CreateAsync(user);
            }
            return user;
        }

        public async Task<ApplicationUser> GetUser(ClaimsPrincipal principal)
        {
            return  principal == null ? null : await _userManager.GetUserAsync(principal);
        }

        public async Task<ApplicationUser> GetUserByEmail(string emailAddress)
        {
            return string.IsNullOrEmpty(emailAddress) ? null : await _userManager.FindByEmailAsync(emailAddress);
        }

        public async Task DeteleUser(ApplicationUser user)
        {
            if (user == null)
            {
                return;
            }

            await _userManager.DeleteAsync(user);
        }

        public async Task AddLoginIfDoesNotExist(ApplicationUser user, UserLoginInfo login)
        {
            var logins = await _userManager.GetLoginsAsync(user);
            if (logins == null ||
                !logins.Any(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey))
            {
                await _userManager.AddLoginAsync(user, login);
            }
        }
    }
}