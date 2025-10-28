using ApplicationService.Repositories;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private const string LUA_SCRIPT = @"
        -- keys[1] => key
        -- ARGV[1] => windowMilliseconds
        local current = redis.call('INCR', KEYS[1])
        if tonumber(current) == 1 then
            redis.call('PEXPIRE', KEYS[1], ARGV[1])
        end
        local ttl = redis.call('PTTL', KEYS[1])
        return {current, ttl}
    ";

        private readonly LoadedLuaScript _prepared;

        public RateLimitService(IConnectionMultiplexer multiplexer)
        {
            _redis = multiplexer;
            _db = _redis.GetDatabase();

            var server = _redis.GetServer(_redis.GetEndPoints()[0]);
            var script = LuaScript.Prepare(LUA_SCRIPT);
            _prepared = script.Load(server);
        }

        public async Task<(bool Allowed, long Current, long Remaining, long ResetSeconds)> TryAcquireAsync(string key, int permitLimit, TimeSpan window)
        {
            if (string.IsNullOrWhiteSpace(key)) key = "anonymous";

            var windowMs = (long)window.TotalMilliseconds;

            RedisResult res;
            try
            {
                // Execute prepared script against a server (server may be null in some setups; fall back to Eval)
                var server = _redis.GetServer(_redis.GetEndPoints()[0]);
                if (server != null)
                {
                    res = await _db.ScriptEvaluateAsync(LUA_SCRIPT, new RedisKey[] { key }, new RedisValue[] { windowMs });
                }
                else
                {
                    res = await _db.ScriptEvaluateAsync(LUA_SCRIPT, new RedisKey[] { key }, new RedisValue[] { windowMs });
                }
            }
            catch
            {
                // In case Redis fails, fail-open (allow) to avoid DoS of your API due to Redis outage.
                return (true, 0, permitLimit, (long)window.TotalSeconds);
            }

            var arr = (RedisResult[])res;
            var current = (long)arr[0];
            var pttl = (long)arr[1]; // ms until expiration, -1 or -2 possible

            long resetSeconds = pttl > 0 ? (pttl + 999) / 1000 : (long)window.TotalSeconds;
            long remaining = permitLimit - current;
            bool allowed = current <= permitLimit;

            if (remaining < 0) remaining = 0;

            return (allowed, current, remaining, resetSeconds);
        }
    }
}
