using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ProjectInformatics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ProjectInformatics.Database
{
    public class ApplicationContext : DbContext, IDbService
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserSubscription> UserSubscriptions { get; set; }
        IGridFSBucket gridFS;   // файловое хранилище
        //IMongoCollection<Message> Messages; // коллекция в базе данных
        public DbSet<Message> Messages { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            //// строка подключения
            //string connectionString = "mongodb://localhost:27017/subscriptions";
            //var connection = new MongoUrlBuilder(connectionString);
            //// получаем клиента для взаимодействия с базой данных
            //MongoClient client = new MongoClient(connectionString);
            //// получаем доступ к самой базе данных
            //IMongoDatabase database = client.GetDatabase(connection.DatabaseName);
            //// получаем доступ к файловому хранилищу
            //gridFS = new GridFSBucket(database);
            //// обращаемся к коллекции Products
            //Messages = database.GetCollection<Message>("Messages");
        }

        public void AddServiceToUser(Subscription subscription, string userEmail)
        {
            Subscriptions.Add(subscription);
            SaveChanges();
            UserSubscriptions.Add(new UserSubscription() {
                UserId = GetUser(userEmail).Id,
                SubscriptionId = subscription.Id });
            SaveChanges();
        }

        public List<Subscription> GetSubscriptions(string userEmail, string order = null)
        {
            var userId = GetUser(userEmail).Id;
            var subsIds = UserSubscriptions
                .Where(x => x.UserId == userId)
                .Select(x => x.SubscriptionId);
            var subs = Subscriptions
                .Where(s => subsIds.Any(id => id == s.Id));
            foreach (var s in subs)
                while ((s.LastPayment + TimeSpan.FromDays(s.Period)).Date < DateTime.Today)
                    s.LastPayment += TimeSpan.FromDays(s.Period);
            SaveChanges();
            if (order == "descendingDate")
                return subs.ToList().OrderByDescending(x => x.LastPayment + TimeSpan.FromDays(x.Period)).ToList();
            if (order == "ascendingName")
                return subs.ToList().OrderBy(x => x.Name).ToList();
            if (order == "descendingName")
                return subs.ToList().OrderByDescending(x => x.Name).ToList();
            return subs.ToList().OrderBy(x => x.LastPayment + TimeSpan.FromDays(x.Period)).ToList();
        }

        public void UpdateUserCategory(string userEmail, int categoryId)
        {
            GetUser(userEmail).CategoryId = categoryId;
            SaveChanges();
        }

        public int GetUserCategory(string userEmail)
        {
            return GetUser(userEmail).CategoryId;
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
            User support = new User { Id = 1000, Email = "suppport@mail.ru", Password = "support", RoleId = 1};
            var noCategory = new Category() { Id = 1, Type = "NoCategory" };
            var standardCategory = new Category() { Id = 2, Price = 15, Type = "Standard" };
            
            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            modelBuilder.Entity<Category>().HasData(new Category[] { noCategory, standardCategory });
            modelBuilder.Entity<User>().HasData(new User[] { support });
            base.OnModelCreating(modelBuilder);
        }

        public List<User> GetUsers()
        {
            return Users.ToList();
        }

        //public List<Message> GetMessages()
        //{
        //    return Messages.AsQueryable().ToList();
        //}

        //public void AddMessage(Message message)
        //{
        //    Messages.InsertOne(message);
        //}

        public List<Message> GetMessages()
        {
            return Messages.ToList();
        }

        public void AddMessage(Message message)
        {
            Messages.Add(message);
            SaveChanges();
        }

        public User GetUser(string email, string password = null)
        {
            return Users.FirstOrDefault(x => x.Email == email
                || password != null && x.Email == email && x.Password == password);
        }

        public Task<List<User>> GetUsersAsync()
        {
            return Users.ToListAsync();
        }

        public void AddUsers(params User[] users)
        {
            Users.AddRange(users);
            SaveChanges();
        }

        public Task<int> AddUserAsync(User user)
        {
            Users.Add(user);
            return SaveChangesAsync();
        }

        public Task<User> GetUserAsync(string email, string password = null)
        {
            return Users.FirstOrDefaultAsync(x => x.Email == email
                || password != null && x.Email == email && x.Password == password);
        }

        public Task<Role> GetRole(string name)
        {
            return Roles.FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}
