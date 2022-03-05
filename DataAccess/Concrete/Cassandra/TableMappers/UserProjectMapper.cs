using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Utilities.IoC;
using Entities.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.TableMappers;

public class UserProjectMapper: Mappings
{
    public UserProjectMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<UserProject>()
            .TableName("user_projects")
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .PartitionKey("id", "status")
            .ClusteringKey(new Tuple<string, SortOrder>("created_at", SortOrder.Descending))
            .Column(u => u.Id, cm => cm.WithName("id").WithDbType(typeof(long)))
            .Column(u => u.ProjectId, cm => cm.WithName("project_id").WithDbType(typeof(long)))
            .Column(u => u.UserId, cm => cm.WithName("user_id").WithDbType(typeof(long)))
            .Column(u => u.CreatedAt, cm => cm.WithName("created_at").WithDbType(typeof(DateTime)))
            .Column(u => u.Status, cm => cm.WithName("status").WithDbType(typeof(bool)));
    }
}