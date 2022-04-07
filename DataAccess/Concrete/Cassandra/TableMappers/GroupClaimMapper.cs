using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Utilities.IoC;
using Entities.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.TableMappers;

public class GroupClaimMapper : Mappings
{
    public GroupClaimMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings =
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<GroupClaim>()
            .TableName("group_claims")
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .PartitionKey("claim_id", "group_id", "status")
            .ClusteringKey(new Tuple<string, SortOrder>("created_at", SortOrder.Descending))
            .Column(u => u.Id, cm => cm.WithName("id").WithDbType(typeof(long)))
            .Column(u => u.ClaimId, cm => cm.WithName("claim_id").WithDbType(typeof(long)))
            .Column(u => u.GroupId, cm => cm.WithName("group_id").WithDbType(typeof(long)))
            .Column(u => u.CreatedAt, cm => cm.WithName("created_at").WithDbType(typeof(DateTimeOffset)))
            .Column(u => u.Status, cm => cm.WithName("status").WithDbType(typeof(bool)));
    }
}