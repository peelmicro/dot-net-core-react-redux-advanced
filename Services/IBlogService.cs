using System.Threading.Tasks;
using NetCoreReactReduxAdvanced.Models.Blog;
using MongoDB.Bson;

namespace NetCoreReactReduxAdvanced.Services
{
    public interface IBlogService
    {
        Task<string> Find(ObjectId userId);
        Task<string> FindOne(ObjectId userId, ObjectId blogId);
        Task<string> Create(PostBlog postBlog, ObjectId userId);
    }
}
