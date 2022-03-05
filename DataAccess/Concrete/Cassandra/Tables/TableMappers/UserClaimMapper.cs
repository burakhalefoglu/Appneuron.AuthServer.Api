using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.Tables.TableMappers;

public class UserClaimMapper: Mappings
{
    public UserClaimMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<UserClaim>()
            .TableName(CassandraTables.UserClaim)
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .Column(u => u.Id, cm => cm.WithName("id"))
            .Column(u => u.ClaimId, cm => cm.WithName("claim_id"))
            .Column(u => u.UsersId, cm => cm.WithName("user_id"))
            .Column(u => u.Status, cm => cm.WithName("status"));
    }
}