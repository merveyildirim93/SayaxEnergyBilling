using Microsoft.Extensions.Options;
using StackExchange.Redis;

public class RedisContext
{
    private readonly ConnectionMultiplexer _muxer;
    public IDatabase Db => _muxer.GetDatabase();

    public RedisContext(IOptions<RedisSettings> settings)
    {
        var cfg = new ConfigurationOptions
        {
            EndPoints = { { settings.Value.Host, settings.Value.Port } },
            Password = settings.Value.Password, 
            Ssl = settings.Value.Ssl,
            AbortOnConnectFail = settings.Value.AbortConnect,
            ConnectTimeout = 10000
        };

        _muxer = ConnectionMultiplexer.Connect(cfg);
    }

    public class RedisSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }
        public bool Ssl { get; set; }
        public bool AbortConnect { get; set; }
    }
}
