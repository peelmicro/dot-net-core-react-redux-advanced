using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using ServiceStack.Redis;

namespace NetCoreReactReduxAdvanced.Services
{
    public class MongoDbService: IMongoDbService
    {
        private readonly IRedisClient _redis;
        private readonly TimeSpan? _expiration;

        public MongoDbService(IRedisClientsManager redis, IConfiguration configuration)
        {
            _redis = redis.GetClient();
            var expirationValue  = configuration["RedisSettings:Expiration"];
            _expiration = string.IsNullOrEmpty(expirationValue)
                ? (TimeSpan?) null
                : TimeSpan.FromSeconds(Convert.ToDouble(expirationValue));
        }
        public async Task<string> ToJsonAsync<T,TP>(IFindFluent<T,TP> cursor, bool useCache = false, string setKey = null)
        {
            object result;
            if (!useCache) {
                result = await cursor.ToListAsync();
                return JsonConvert.SerializeObject(result);
            }

            var key = typeof(T).Name + "_" + cursor;
            var redisKey = (setKey ?? "") + key;
            var cachedValue = _redis.GetValue(redisKey);
            if (!string.IsNullOrEmpty(cachedValue))
            {
                return cachedValue;
            }
            result = await cursor.ToListAsync();
            var value = JsonConvert.SerializeObject(result);
            if (_expiration != null)
            {
                _redis.SetValue(redisKey, value, (TimeSpan) _expiration);

            }
            else
            {
                _redis.SetValue(redisKey, value);
            }

            if (!string.IsNullOrEmpty(setKey))
            {
                _redis.AddItemToSet(setKey, redisKey);
            }
            return value;
        }

        public void RemoveCacheFromSet(string setKey)
        {
            var items = _redis.GetAllItemsFromSet(setKey);
            foreach (var item in items)
            {
                _redis.RemoveItemFromSet(setKey, item);
                _redis.Remove(item);
            }
        }
    }
}