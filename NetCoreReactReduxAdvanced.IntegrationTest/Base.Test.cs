using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Microsoft.AspNetCore.Identity;
using NetCoreReactReduxAdvanced.Models;
using NetCoreReactReduxAdvanced.Services;

namespace NetCoreReactReduxAdvanced.IntegrationTest
{
    public class BaseTest:  IDisposable
    {
        protected Browser Browser { get; set; }
        protected Page Page { get; private set; }
        protected TestServer TestServer { get; private set; }
        protected  HttpClient HttpClient { get; private set; }
        protected ApplicationUser NewUser { get; private set; }
        protected IUserService UserService { get; private set; }

        public BaseTest()
        {
            Task.Run(InitializeAsync).Wait();
        }

        public void Dispose()
        {
            Task.Run(DisposeAsync).Wait();
        }
        public async Task InitializeAsync()
        {
            var mainPath = Path.GetFullPath("../../../../NetCoreReactReduxAdvanced");

            TestServer = new TestServer(
                new WebHostBuilder()
                    .UseStartup<Startup>()
                    .UseContentRoot(mainPath)
                    .UseEnvironment("Development")
                    .ConfigureAppConfiguration((context, config) =>
                    {
                        config.SetBasePath(mainPath);
                        config.AddJsonFile("appsettings.json");
                    })
                );
            HttpClient = TestServer.CreateClient();
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Browser = await Puppeteer.LaunchAsync(new LaunchOptions{Headless = true});
            Page = await Browser.NewPageAsync();
            await Page.GoToAsync("https://localhost:5001/");            
        }

        public async Task DisposeAsync()
        {
            await RemoveUser();
            TestServer.Dispose();
            await Browser.CloseAsync();
            await Page.CloseAsync();
        }

		#region protected help methods
        protected async Task ClickAndWaitForNavigation(string selector) 
        {
            await Task.WhenAll(
                Page.ClickAsync(selector),
                Page.WaitForNavigationAsync()); 
        }
        protected async Task<string> GetContentOf(string selector)
        {
            var attempts = 0;
            while (attempts < 3)
            {
                attempts++;
                try
                {
                    var text = await Page.QuerySelectorAsync(selector)
                        .EvaluateFunctionAsync<string>("el => el.innerHTML");
                    return text;
                }
                catch (Exception)
                {
                    await Task.Delay(1000);
                }
            }
            return null;
        }
		protected async Task Login() 
        {
            var text = await GetContentOf(".right a");                
            if (text == "Login With Google") 
            {            
                var cookie = await GetCookie();
                await Page.SetCookieAsync(new CookieParam {
                    Name = ".AspNetCore.Identity.Application",
                    Value = cookie
                });
                await Task.WhenAll(
                    Page.GoToAsync("https://localhost:5001/blogs"),
                    Page.WaitForNavigationAsync()); 
            }
        }

        protected async Task<dynamic> Get(params object[] args)
//        protected async Task<dynamic> Get(string path)
        {
            var result = await Page.EvaluateFunctionAsync(@"
              (_path) => {
                return fetch(_path, {
                  method: 'GET',
                  credentials: 'same-origin',
                  headers: {
                    'Content-Type': 'application/json'
                  }
                })
                  .then(res => res.json());
              }
            ", args);
//            ", path);
            return result;
        }

        protected async Task<dynamic> Post(params object[] args)
        //protected async Task<dynamic> Post(string path, dynamic data)
        {
            var result = await Page.EvaluateFunctionAsync(@"
              (_path, _data) => {
                return fetch(_path, {
                  method: 'POST',
                  credentials: 'same-origin',
                  headers: {
                    'Content-Type': 'application/json'
                  },
                  body: JSON.stringify(_data)
                })
                  .then(res => res.json());
              }
            ", args);
//            ", path, data);

            return result;
        }

        private delegate dynamic Request(params object[] args);

        protected async Task<List<dynamic>> ExecRequests(IEnumerable<object> requests)
        {
            var results = new List<dynamic>();
            foreach (dynamic request in requests)
            {
                Request rq = null;
                switch (request.method)
                {
                    case "get":
                        rq = Get;
                        break;
                    case "post":
                        rq = Post;
                        break;
                }

                if (rq == null) continue;
                var result = await rq(request.path, request.data);
                results.Add(result);
            }

            return results;
        }
        #endregion

        #region private help methods
        private async Task<string> GetCookie()
        {
            await CreateNewUser();
            if (NewUser == null)
            {
                return null;
            }
            var authenticationMethod = NewUser.Logins.FirstOrDefault()?.LoginProvider ?? "Google";
            var claims = new[]
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", NewUser.Id.ToString()),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name", NewUser.UserName),
                new Claim("AspNet.Identity.SecurityStamp", NewUser.SecurityStamp),
                new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/authenticationmethod", authenticationMethod)
            };
            const string cookieSchema = "Identity.Application";
            var myService = TestServer.Host.Services.GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>();
            var dataProtector = myService.CurrentValue.DataProtectionProvider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", cookieSchema, "v2");
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, cookieSchema));
            var ticket = new AuthenticationTicket(principal, cookieSchema);
            var ticketDataFormat = new TicketDataFormat(dataProtector);
            var cookie = ticketDataFormat.Protect(ticket);
            return cookie;
        }

        private async Task CreateNewUser(string provider = "Google")
        {
            if (UserService == null)
            {
                UserService = TestServer.Host.Services.GetRequiredService<IUserService>();
            }

            var user = await UserService.CreateUserFromEmail();
            var userLoginInfo = new UserLoginInfo(provider, Guid.NewGuid().ToString("N"), provider);
            await UserService.AddLoginIfDoesNotExist(user, userLoginInfo);
            NewUser = await UserService.GetUserByEmail(user.Email);
        }

        private async Task RemoveUser()
        {
            if (UserService == null || NewUser == null)
            {
                return;
            }
            await UserService.DeteleUser(NewUser);
        }
        #endregion
    }
}