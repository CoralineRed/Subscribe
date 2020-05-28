using ProjectInformatics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInformatics.Database
{
    public interface IDbService
    {
        void AddServiceToUser(Subscription subscription, string userEmail);
        List<Subscription> GetSubscriptions(string userEmail, string order = null);
        void UpdateUserCategory(string userEmail, int categoryId);
        int GetUserCategory(string userEmail);
        List<User> GetUsers();
        List<Message> GetMessages();
        void AddMessage(Message message);
        User GetUser(string email, string password = null);
        Task<User> GetUserAsync(string email, string password = null);
        Task<List<User>> GetUsersAsync();
        void AddUsers(params User[] users);
        Task<int> AddUserAsync(User user);
    }
}
