using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using NetCoreReactReduxAdvanced.Models;

namespace NetCoreReactReduxAdvanced.Services
{
    public interface IUserService
    {
        Task<ApplicationUser> CreateUserFromEmail(string emailAddress = null);
        Task<ApplicationUser> GetUser(ClaimsPrincipal principal);
        Task<ApplicationUser> GetUserByEmail(string emailAddress);
        Task DeteleUser(ApplicationUser user);
        Task AddLoginIfDoesNotExist(ApplicationUser user, UserLoginInfo login);
    }
}
