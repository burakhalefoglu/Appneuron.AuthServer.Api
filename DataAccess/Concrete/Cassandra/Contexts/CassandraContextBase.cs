using Core.DataAccess.Cassandra.Configurations;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Concrete.Cassandra.Contexts
{
    public abstract class CassandraContexts
    {
        public readonly CassandraConnectionSettings CassandraConnectionSettings;

        protected CassandraContexts(IConfiguration configuration)
        {
            CassandraConnectionSettings = configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        }
    }
} 