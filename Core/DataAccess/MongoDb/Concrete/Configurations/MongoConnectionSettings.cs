using MongoDB.Driver;

namespace Core.DataAccess.MongoDb.Concrete.Configurations
{
    public class MongoConnectionSettings
    {
        public MongoConnectionSettings(MongoClientSettings mongoClientSettings)
        {
            MongoClientSettings = mongoClientSettings;
        }

        public MongoConnectionSettings()
        {
        }

        /// <summary>
        ///     To be set if the MongoClientSetting class is to be used.
        /// </summary>
        private MongoClientSettings MongoClientSettings { get; }

        public string Host { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }

        public MongoClientSettings GetMongoClientSettings()
        {
            return MongoClientSettings;
        }
    }
}