using ProjectInformatics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInformatics.Services
{
    public interface ICacheService
    {
        void Initialize();
        Task<IEnumerable<User>> GetUsers();
        Task AddUser(User user);
        Task<User> GetUser(string email, string password);
    }
}
