
using Xunit;

namespace NetCoreReactReduxAdvanced.IntegrationTest
{
//    [Collection("Sequential")]
    public class HeaderTest:  BaseTest
    {

        [Fact]
        public async void TheHeaderHasTheCorrectText() {
            var text = await GetContentOf("a.brand-logo");
            Assert.Equal("Blogster", text);
        }

        [Fact]
        public async void ClickingLoginStartsOauthFlow() {
            const string selector = ".right a";
            var text = await GetContentOf(selector);
            if (text == "Login With Google") 
            {
                await ClickAndWaitForNavigation(selector);
                Assert.Matches(@"/accounts\.google\.com/", Page.Url);
            } 
            else 
            {
                Assert.Equal("My Blogs", text);
            }
        }
        [Fact]
        public async void WhenSignedInShowsLogoutButton()
        {
            await Login();
            const string selector = "a[href='/auth/logout']";
            var text = await GetContentOf(selector);                
            Assert.Equal("Logout", text);
            await ClickAndWaitForNavigation(selector);
        }

    }
}
