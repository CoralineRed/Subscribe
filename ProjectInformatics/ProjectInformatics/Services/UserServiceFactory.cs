using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProjectInformatics.Logging;
using System;

namespace ProjectInformatics.Services
{
    public class UserServiceFactory
    {
        private readonly ILogger _logger;

        public UserServiceFactory(ILogger logger)
        {
            _logger = logger;
        }

        public UserService CreateUserService(ApplicationContext context, IMemoryCache memoryCache)
        {
            return LoggingAdvice<UserService>.Create(
                new UserService(context, memoryCache),
                s => _logger.LogInformation("Info:" + s),
                s => _logger.LogInformation("Error:" + s),
                o => o?.ToString());
        }
    }
}
