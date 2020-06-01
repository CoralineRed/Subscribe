using ProjectInformatics.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectInformatics.Database;

namespace ProjectInformatics.Services
{
    public class UserService
    {
        private IDbService db;
        private IMemoryCache cache;

        public UserService(IDbService context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public void Initialize()
        {
            if (!db.GetUsers().Any())
                db.AddUsers(
                    new User { Name = "Tom", Email = "tom@gmail.com"},
                    new User { Name = "Alice", Email = "alice@yahoo.com"},
                    new User { Name = "Sam", Email = "sam@online.com"}
                );
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await db.GetUsersAsync();
        }

        public async Task AddUser(User user)
        {
            Role userRole = await db.GetRole("user");
            if (userRole != null)
                user.Role = userRole;
            int n = await db.AddUserAsync(user);
            if (n > 0)
            {
                cache.Set(user.Id, user, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
        }

        public async Task<User> GetUser(string email, string password)
        {
            User user = await db.GetUserAsync(email, password);
            if (user != null && !cache.TryGetValue(user.Id, out User _))
            {
                    cache.Set(user.Id, user,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
            }
            return user;
        }
    }
}