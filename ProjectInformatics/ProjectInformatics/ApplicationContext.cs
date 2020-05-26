using Microsoft.EntityFrameworkCore;
using ProjectInformatics.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ProjectInformatics
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public void AddServiceToUser(Subscription subscription, string userEmail)
        {
            Subscriptions.Add(subscription);
            SaveChanges();
            var userId = Users.Where(u => u.Email == userEmail).FirstOrDefault().Id;
            UserSubscriptions.Add(new UserSubscription() { UserId = userId, SubscriptionId = subscription.Id });
            SaveChanges();
        }

        public List<Subscription> GetSubscriptions(string userEmail)
        {
            var userId = Users.Where(u => u.Email == userEmail).FirstOrDefault().Id;
            var subsIds = UserSubscriptions
                .Where(x => x.UserId == userId)
                .Select(x => x.SubscriptionId);
            return Subscriptions
                .Where(s => subsIds.Any(id => id == s.Id))
                .ToList();

        }
    }
}
