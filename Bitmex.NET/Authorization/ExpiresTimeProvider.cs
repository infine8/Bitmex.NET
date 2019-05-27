using System;

namespace Bitmex.NET.Authorization
{
    public interface IExpiresTimeProvider
    {
        long Get();
    }

    public class ExpiresTimeProvider : IExpiresTimeProvider
    {
        private const int LifetimeSeconds = 300;

        private static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public long Get()
        {
            return (long)(DateTime.UtcNow - EpochTime).TotalSeconds + LifetimeSeconds;
        }
    }
}
