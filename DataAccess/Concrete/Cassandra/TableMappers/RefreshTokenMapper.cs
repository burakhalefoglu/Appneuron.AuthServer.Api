using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Utilities.IoC;
using Entities.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.TableMappers;

public class RefreshTokenMapper: Mappings
{
    public RefreshTokenMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<RefreshToken>()
            .TableName("refresh_tokens") 
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .PartitionKey("user_id")
            .ClusteringKey(new Tuple<string, SortOrder>("created_at", SortOrder.Ascending))
            .Column(u => u.Id, cm => cm.WithName("id").WithDbType(typeof(long)))
            .Column(u => u.UserId, cm => cm.WithName("user_id").WithDbType(typeof(long)))
            .Column(u => u.Value, cm => cm.WithName("value").WithDbType(typeof(string)))
            .Column(u => u.Status, cm => cm.WithName("status").WithDbType(typeof(bool)))
            .Column(u => u.CreatedAt, cm => cm.WithName("created_at").WithDbType(typeof(DateTimeOffset)));
        
    }
}
