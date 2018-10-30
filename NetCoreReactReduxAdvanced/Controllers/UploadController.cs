using System;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetCoreReactReduxAdvanced.Models;
using Newtonsoft.Json.Linq;

namespace NetCoreReactReduxAdvanced.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private const string BucketName = "the-blog-dev-bucket";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAmazonS3 _s3Client;

        public UploadController(UserManager<ApplicationUser> userManager, IAmazonS3 s3Client)
        {
            _userManager = userManager;
            _s3Client = s3Client;
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var objectKey = $@"{user.Id}/{Guid.NewGuid().ToString()}.jpeg";
            
            var s3Request = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = objectKey,
                Expires = DateTime.Now.AddMinutes(5),
                Verb = HttpVerb.PUT,
                ContentType = "image/jpeg"
            };
            var urlString = _s3Client.GetPreSignedURL(s3Request);

            dynamic response = new JObject();
            response.key = objectKey;
            response.url = urlString;
            return Ok(response);
        }
    }
}