using ProjectInformatics.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInformatics.Services
{
    public class UserService
    {
        private ApplicationContext db;
        private IMemoryCache cache;
        public UserService(ApplicationContext context, IMemoryCache memoryCache)
        {
            db = context;
            cache = memoryCache;
        }

        public void Initialize()
        {
            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new User { Name = "Tom", Email = "tom@gmail.com"},
                    new User { Name = "Alice", Email = "alice@yahoo.com"},
                    new User { Name = "Sam", Email = "sam@online.com"}
                );
                db.SaveChanges();
            }
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await db.Users.ToListAsync();
        }

        public async Task AddUser(User user)
        {
            db.Users.Add(user);
            int n = await db.SaveChangesAsync();
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
            User userr = null;
            User user = await db.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
            if (!cache.TryGetValue(user.Id, out userr))
            {
                if (user != null)
                {
                    cache.Set(user.Id, user,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return user;
        }
    }
}