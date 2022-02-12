using Microsoft.Extensions.Configuration;

namespace DataAccess.Concrete.Cassandra.Contexts
{
    public class CassandraContext: CassandraContextBase
    {
        public CassandraContext(IConfiguration configuration) : base(configuration)
        {
        }
        
    }
}