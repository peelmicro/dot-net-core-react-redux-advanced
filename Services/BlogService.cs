using System.Threading.Tasks;
using MongoDbGenericRepository;
using MongoDB.Bson;
using MongoDB.Driver;
using NetCoreReactReduxAdvanced.Models.Blog;
using Newtonsoft.Json;


namespace NetCoreReactReduxAdvanced.Services
{
    public class BlogService : IBlogService
    {
        private readonly IMongoCollection<Blog> _blogCollection;
        private readonly IMongoDbService _mongoDbService;


        public BlogService(IMongoDbContext mongoDbContext, IMongoDbService mongoDbService)
        {
            _blogCollection = mongoDbContext.GetCollection<Blog, ObjectId>();
            _mongoDbService = mongoDbService;
        }

        public async Task<string> Find(ObjectId userId)
        {
            var cursor = _blogCollection.Find(col => col.User == userId);            
            return await _mongoDbService.ToJsonAsync(cursor, true, userId.ToString());
        }
        public async Task<string> FindOne(ObjectId userId, ObjectId blogId)
        {
            var blog = await _blogCollection.Find(col => col.Id == blogId && col.User == userId).SingleOrDefaultAsync();
            return JsonConvert.SerializeObject(blog);
        }

        public async Task<string> Create(PostBlog postBlog, ObjectId userId)
        {
            var blog = new Blog(postBlog, userId);
            await _blogCollection.InsertOneAsync(blog);
            _mongoDbService.RemoveCacheFromSet(userId.ToString());
            return JsonConvert.SerializeObject(blog);
        }
    }
}
