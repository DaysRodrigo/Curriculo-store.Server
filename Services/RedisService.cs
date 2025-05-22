using StackExchange.Redis;

namespace Curriculo_store.Server.Services
{
    public class RedisService
    {
        private readonly IDatabase _database;

        public RedisService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task SetTokenAsync(string token, string value, TimeSpan? expiration = null)
        {
            await _database.StringSetAsync(token, value, expiration ?? TimeSpan.FromSeconds(1));
        }

        public async Task<string?> GetTokenAsync(string token)
        {
            return await _database.StringGetAsync(token);
        }

        public async Task<bool> DeleteTokenAsync(string token)
        {
            return await _database.KeyDeleteAsync(token);
        }

        public async Task<bool> TokenExistAsync(string token)
        {
            return await _database.KeyExistsAsync(token);
        }
    }
}