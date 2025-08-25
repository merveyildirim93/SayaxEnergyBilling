using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

public class RedisContext
{
    private readonly ConnectionMultiplexer _muxer;
    public IDatabase Db => _muxer.GetDatabase();

    public RedisContext(IConfiguration configuration)
    {
        var redisConfig = configuration.GetSection("Redis");
        string host = redisConfig["Host"];
        int port = int.Parse(redisConfig["Port"] ?? "11915");
        string user = redisConfig["User"];
        string password = redisConfig["Password"];
        bool ssl = bool.Parse(redisConfig["Ssl"] ?? "true");
        bool abortConnect = bool.Parse(redisConfig["AbortConnect"] ?? "false");

        var config = new ConfigurationOptions
        {
            EndPoints = { { host, port } },
            User = user,
            Password = password,
            Ssl = ssl,
            AbortOnConnectFail = abortConnect
        };

        _muxer = ConnectionMultiplexer.Connect(config);
    }
}
