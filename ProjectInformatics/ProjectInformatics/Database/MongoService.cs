using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using ProjectInformatics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectInformatics.Database
{
    public class MongoService
    {
        IGridFSBucket gridFS;   // файловое хранилище
        IMongoCollection<Message> Messages; // коллекция в базе данных

        public MongoService()
        {
            // строка подключения
            string connectionString = "mongodb://localhost:27017/mobilestore";
            var connection = new MongoUrlBuilder(connectionString);
            // получаем клиента для взаимодействия с базой данных
            MongoClient client = new MongoClient(connectionString);
            // получаем доступ к самой базе данных
            IMongoDatabase database = client.GetDatabase(connection.DatabaseName);
            // получаем доступ к файловому хранилищу
            gridFS = new GridFSBucket(database);
            // обращаемся к коллекции Products
            Messages = database.GetCollection<Message>("Messages");
        }
    }
}
