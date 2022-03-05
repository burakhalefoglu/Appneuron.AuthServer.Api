using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.Tables.TableMappers;

public class LanguageMapper: Mappings
{
    public LanguageMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<Language>()
            .TableName(CassandraTables.Language)
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .Column(u => u.Id, cm => cm.WithName("id"))
            .Column(u => u.Code, cm => cm.WithName("code"))
            .Column(u => u.Name, cm => cm.WithName("name"))
            .Column(u => u.Status, cm => cm.WithName("status"));
    }
}