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
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            if (Categories.Count() == 0)
                Categories.AddRange(
                    new Category() { Type = "NoCategory" },
                    new Category() { Price = 15, Type = "Standard" });
        }

        public void AddServiceToUser(Subscription subscription, string userEmail)
        {
            Subscriptions.Add(subscription);
            SaveChanges();
            var userId = Users.FirstOrDefault(u => u.Email == userEmail).Id;
            UserSubscriptions.Add(new UserSubscription() { UserId = userId, SubscriptionId = subscription.Id });
            SaveChanges();
        }

        public List<Subscription> GetSubscriptions(string userEmail)
        {
            var userId = Users.FirstOrDefault(u => u.Email == userEmail).Id;
            var subsIds = UserSubscriptions
                .Where(x => x.UserId == userId)
                .Select(x => x.SubscriptionId);
            return Subscriptions
                .Where(s => subsIds.Any(id => id == s.Id))
                .ToList();
        }

        public void UpdateUserCategory(string userEmail, int categoryId)
        {
            var user = Users.FirstOrDefault(u => u.Email == userEmail);
            user.CategoryId = categoryId;
            SaveChanges();
        }

        public int GetUserCategory(string userEmail)
        {
            return Users.FirstOrDefault(x => x.Email == userEmail).CategoryId;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            string adminEmail = "admin123@mail.ru";
            string adminPassword = "12345678";

            // добавляем роли
            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };
            User adminUser = new User { Id = 1, Email = adminEmail, Password = adminPassword, RoleId = adminRole.Id };
            User support = new User { Id = 1000, Email = "suppport@mail.ru", Password = "support", RoleId = 1 };
            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            modelBuilder.Entity<User>().HasData(new User[] { support });
            base.OnModelCreating(modelBuilder);
        }
    }
}
