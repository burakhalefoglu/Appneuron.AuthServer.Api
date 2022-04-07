using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.TableMappers;

public class LogMapper : Mappings
{
    public LogMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings =
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<Log>()
            .TableName("logs")
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .PartitionKey("id")
            .ClusteringKey(new Tuple<string, SortOrder>("time_stamp", SortOrder.Descending))
            .Column(u => u.Id, cm => cm.WithName("id").WithDbType(typeof(long)))
            .Column(u => u.Level, cm => cm.WithName("level").WithDbType(typeof(string)))
            .Column(u => u.MessageTemplate, cm => cm.WithName("message_template").WithDbType(typeof(string)))
            .Column(u => u.TimeStamp, cm => cm.WithName("time_stamp").WithDbType(typeof(DateTimeOffset)))
            .Column(u => u.Exception, cm => cm.WithName("exception").WithDbType(typeof(string)))
            .Column(u => u.Status, cm => cm.WithName("status").WithDbType(typeof(bool)));
    }
}