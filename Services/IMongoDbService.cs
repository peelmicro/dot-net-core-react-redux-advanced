using System.Threading.Tasks;
using MongoDB.Driver;

namespace NetCoreReactReduxAdvanced.Services
{
    public interface IMongoDbService
    {
        Task<string> ToJsonAsync<T,TP>(IFindFluent<T,TP> cursor, bool useCache = false, string hashKey = null);
        void RemoveCacheFromSet(string setKey);
    }
}
