using System.Reflection.Metadata;
using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using Entities.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.TableMappers;

public class UserMapper: Mappings
{
    public UserMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<User>()
            .TableName("users") 
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .PartitionKey("email", "status")
            .ClusteringKey(new Tuple<string, SortOrder>("id", SortOrder.Ascending))
            .Column(u => u.Id, cm => cm.WithName("id").WithDbType(typeof(long)))
            .Column(u => u.Name, cm => cm.WithName("name").WithDbType(typeof(string)))
            .Column(u => u.Email, cm => cm.WithName("email").WithDbType(typeof(string)))
            .Column(u => u.RecordDate, cm => cm.WithName("record_date").WithDbType(typeof(DateTimeOffset)))
            .Column(u => u.UpdateContactDate, cm => cm.WithName("update_contact_date").WithDbType(typeof(DateTimeOffset)))
            .Column(u => u.PasswordSalt, cm => cm.WithName("password_salt").WithDbType(typeof(byte[])))
            .Column(u => u.PasswordHash, cm => cm.WithName("password_hash").WithDbType(typeof(byte[])))
            .Column(u => u.ResetPasswordToken, cm => cm.WithName("reset_password_token").WithDbType(typeof(string)))
            .Column(u => u.ResetPasswordExpires, cm => cm.WithName("reset_password_expires").WithDbType(typeof(DateTimeOffset)))
            .Column(u => u.Status, cm => cm.WithName("status").WithDbType(typeof(bool)));
    }
}
