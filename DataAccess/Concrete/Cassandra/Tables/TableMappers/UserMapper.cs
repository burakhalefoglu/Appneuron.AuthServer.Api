using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.Tables.TableMappers;

public class UserMapper: Mappings
{
    public UserMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<User>()
            .TableName(CassandraTables.User)
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .Column(u => u.Id, cm => cm.WithName("id"))
            .Column(u => u.Name, cm => cm.WithName("name"))
            .Column(u => u.Email, cm => cm.WithName("email"))
            .Column(u => u.RecordDate, cm => cm.WithName("record_date"))
            .Column(u => u.UpdateContactDate, cm => cm.WithName("update_contact_date"))
            .Column(u => u.PasswordSalt, cm => cm.WithName("password_salt"))
            .Column(u => u.PasswordHash, cm => cm.WithName("password_hash"))
            .Column(u => u.ResetPasswordToken, cm => cm.WithName("reset_password_token"))
            .Column(u => u.ResetPasswordExpires, cm => cm.WithName("reset_password_expires"))
            .Column(u => u.Status, cm => cm.WithName("status"));
    }
}