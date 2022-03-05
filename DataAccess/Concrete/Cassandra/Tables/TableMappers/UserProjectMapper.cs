using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Utilities.IoC;
using Entities.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.Tables.TableMappers;

public class UserProjectMapper: Mappings
{
    public UserProjectMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<UserProject>()
            .TableName(CassandraTables.UserProject)
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .Column(u => u.Id, cm => cm.WithName("id"))
            .Column(u => u.ProjectId, cm => cm.WithName("project_id"))
            .Column(u => u.UserId, cm => cm.WithName("user_id"))
            .Column(u => u.Status, cm => cm.WithName("status"));
    }
}