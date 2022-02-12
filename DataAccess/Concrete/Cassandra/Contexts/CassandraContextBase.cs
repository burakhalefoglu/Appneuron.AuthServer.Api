using Core.DataAccess.Cassandra.Configurations;
using Microsoft.Extensions.Configuration;

namespace DataAccess.Concrete.Cassandra.Contexts
{
    public abstract class CassandraContextBase
    {
        public readonly CassandraConnectionSettings CassandraConnectionSettings;

        protected CassandraContextBase(IConfiguration configuration)
        {
            CassandraConnectionSettings = configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        }
    }
} 