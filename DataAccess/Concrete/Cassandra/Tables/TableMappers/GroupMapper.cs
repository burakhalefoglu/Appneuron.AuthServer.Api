using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.Tables.TableMappers;

public class GroupMapper:Mappings
{
    public GroupMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<Group>()
            .TableName(CassandraTables.Group)
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .Column(u => u.Id, cm => cm.WithName("id"))
            .Column(u => u.GroupName, cm => cm.WithName("group_name"))
            .Column(u => u.Status, cm => cm.WithName("status"));
    }
}