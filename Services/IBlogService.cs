using System.Threading.Tasks;
using MongodbIdentity.Models;
using MongoDB.Bson;

namespace NetCoreReactReduxAdvanced.Services
{
    public interface IBlogService
    {
        //Task<List<Blog>> Find(ObjectId userId);
        Task<string> Find(ObjectId userId);
        //Task<Blog> FindOne(ObjectId userId, ObjectId blogId);
        Task<string> FindOne(ObjectId userId, ObjectId blogId);
        void Create(PostBlog postBlog, ObjectId userId);
    }
}
