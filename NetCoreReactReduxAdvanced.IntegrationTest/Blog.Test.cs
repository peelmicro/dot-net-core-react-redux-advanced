using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NetCoreReactReduxAdvanced.Models.Blog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace NetCoreReactReduxAdvanced.IntegrationTest
{
    public class BlogTest: BaseTest
    {

        private async Task WhenLoggedIn()
        {
            await Login();
            await ClickAndWaitForNavigation("a.btn-floating");
        }

        private async Task WhenLoggedInAndUsingValidInputs()
        {
            await WhenLoggedIn();
            await Page.TypeAsync(".title input", "My Title");
            await Page.TypeAsync(".content input", "My Content");
            await Page.ClickAsync("form button");
        }

        [Fact]
        public async void WhenLoggedInICanSeeBlogCreateForm()
        {
            await WhenLoggedIn();
            var text = await GetContentOf("form label");
            Assert.Equal("Blog Title", text);
        }

        [Fact]
        public async void  WhenLoggedInAndUsingInvalidInputsTheFormShowsAnErrorMessage() {
            await WhenLoggedIn();
            await Page.ClickAsync("form button");

            var titleError = await GetContentOf(".title .red-text");
            var contentError = await GetContentOf(".content .red-text");
            Assert.Equal("You must provide a value", titleError);
            Assert.Equal("You must provide a value", contentError);
        }

        [Fact]
        public async void  WhenLoggedInAndUsingValidInputsSubmittingTakesUserToReviewScreen()
        {
            await WhenLoggedInAndUsingValidInputs();

            var text = await GetContentOf("form h5");
            Assert.Equal("Please confirm your entries", text);
        }

        [Fact]
        public async void  WhenLoggedInAndUsingValidInputsSubmittingThenSavingAddsBlogToIndexPage()
        {
            await WhenLoggedInAndUsingValidInputs();

            await ClickAndWaitForNavigation("button.green");

            var title = await GetContentOf(".card-title");
            var content = await GetContentOf("p");
            Assert.Equal("My Title", title);
            Assert.Equal("My Content", content);
        }

        [Fact]
        public async void WhenUserIsNotLoggedInUserCannotCreateBlogPosts()
        {
//            This approach works as well            
//            dynamic request = new JObject();
//            request.title = "My Title";
//            request.content = "My Content";
//            var result = await Post("/api/blogs", request);
            var result = await Post("/api/blogs", "{ title: 'My Title', content: 'My Content' }");

            dynamic resultExpected = new JObject();
            resultExpected.error = "You must log in!";

            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public async void WhenUserIsNotLoggedInUserCannotCreateBlogPostsTestServerVersion()
        {
            var response = await HttpClient.PostAsync("/api/blogs"
                , new StringContent(
                    JsonConvert.SerializeObject(new PostBlog
                    {
                        Title = "My Title",
                        Content = "My Content"
                    }), 
                    Encoding.UTF8, 
                    "application/json"));
 
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var result = response.Content.ReadAsAsync<dynamic>().Result;

            dynamic resultExpected = new JObject();
            resultExpected.error = "You must log in!";

            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public async void WhenUserIsNotLoggedInUserCannotGetAListOfPosts()
        {
            var result = await Get("/api/blogs");

            dynamic resultExpected = new JObject();
            resultExpected.error = "You must log in!";

            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public async void WhenUserIsNotLoggedInUserCannotGetAListOPostsTestServerVersion()
        {
            var response = await HttpClient.GetAsync("/api/blogs");
 
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            var result = response.Content.ReadAsAsync<dynamic>().Result;

            dynamic resultExpected = new JObject();
            resultExpected.error = "You must log in!";

            Assert.Equal(resultExpected, result);
        }

        [Fact]
        public async void WhenUserIsNotLoggedInBlogRelatedActionsAreProhibited()
        {
            var actions = new[] {
                new { method = "get", path = "/api/blogs", data = "" },
                new { method = "post", path = "/api/blogs", data = "{ title: 'My Title', content: 'My Content' }" }
            };

            dynamic resultExpected = new JObject();
            resultExpected.error = "You must log in!";

            var results = await ExecRequests(actions);
            foreach (var result in results)
            {
                Assert.Equal(resultExpected, result);
            }
            
        }

    }
}
