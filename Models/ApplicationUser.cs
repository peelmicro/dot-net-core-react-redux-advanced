using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;

namespace NetCoreReactReduxAdvanced.Models
{
    public class ApplicationUser : MongoIdentityUser<ObjectId>
    {
        public ApplicationUser() : base()
        {
        }

        public ApplicationUser(string userName, string email) : base(userName, email)
        {
        }
    }
}