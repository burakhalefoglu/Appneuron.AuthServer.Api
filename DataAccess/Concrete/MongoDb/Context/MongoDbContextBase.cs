using Core.DataAccess.MongoDb.Concrete.Configurations;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Concrete.MongoDb.Context
{
    public abstract class MongoDbContextBase
    {
        public readonly MongoConnectionSettings MongoConnectionSettings;

        protected MongoDbContextBase(IConfiguration configuration)
        {
            MongoConnectionSettings = configuration.GetSection("MongoDbSettings").Get<MongoConnectionSettings>();
        }
    }
}