using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.Tables.TableMappers;

public class OperationClaimMapper: Mappings
{
    public OperationClaimMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<OperationClaim>()
            .TableName(CassandraTables.OperationClaim)
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .Column(u => u.Id, cm => cm.WithName("id"))
            .Column(u => u.Name, cm => cm.WithName("name"))
            .Column(u => u.Alias, cm => cm.WithName("alias"))
            .Column(u => u.Description, cm => cm.WithName("description"))
            .Column(u => u.Status, cm => cm.WithName("status"));
    }
}