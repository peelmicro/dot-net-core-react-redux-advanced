using System.Threading.Tasks;
using MongodbIdentity.Models;
using MongoDbGenericRepository;
using MongoDB.Bson;
using MongoDB.Driver;
using NetCoreReactReduxAdvanced.Utils;
using Newtonsoft.Json;

namespace NetCoreReactReduxAdvanced.Services
{
    public class BlogService: IBlogService
    {
        private readonly IMongoCollection<Blog> _blogCollection;


        public BlogService(IMongoDbContext mongoDbContext)
        {
            _blogCollection = mongoDbContext.GetCollection<Blog, ObjectId>();
        }

        public async Task<string> Find(ObjectId userId)
        {
            var blogs = await _blogCollection.Find(col => col.User == userId).ToListAsync();
            return ChangeNames(blogs);
        }
        public async Task<string> FindOne(ObjectId userId, ObjectId blogId)
        {
            var blog = await _blogCollection.Find(col => col.Id == blogId && col.User == userId).SingleOrDefaultAsync();
            return ChangeNames(blog);
        }

        public async void Create(PostBlog postBlog, ObjectId userId)
        {
            var blog = new Blog(postBlog, userId);
            await _blogCollection.InsertOneAsync(blog);
        }

        //TODO: This should be change some other way. To figure out how ...
        private static string ChangeNames(object value) {
            var jsonResolver = new PropertyRenameAndIgnoreSerializerContractResolver();
            jsonResolver.RenameProperty(typeof(Blog), "Id", "_id");
            jsonResolver.RenameProperty(typeof(Blog), "Title", "title");
            jsonResolver.RenameProperty(typeof(Blog), "Content", "content");
            var serializerSettings = new JsonSerializerSettings {ContractResolver = jsonResolver};
            return JsonConvert.SerializeObject(value, serializerSettings);
        }
    }
}
