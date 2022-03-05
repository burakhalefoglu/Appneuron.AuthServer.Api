using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.Tables.TableMappers;

public class LogMapper: Mappings
{
    public LogMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<Log>()
            .TableName(CassandraTables.Log)
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .Column(u => u.Id, cm => cm.WithName("id"))
            .Column(u => u.Level, cm => cm.WithName("level"))
            .Column(u => u.MessageTemplate, cm => cm.WithName("message_template"))
            .Column(u => u.TimeStamp, cm => cm.WithName("time_stamp"))
            .Column(u => u.Exception, cm => cm.WithName("exception"));
    }
}