﻿using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.Cassandra.TableMappers;

public class UserGroupMapper: Mappings
{
    public UserGroupMapper()
    {
        var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
        var cassandraConnectionSettings = 
            configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
        For<UserGroup>()
            .TableName("user_groups")
            .KeyspaceName(cassandraConnectionSettings.Keyspace)
            .PartitionKey("id", "status")
            .ClusteringKey(new Tuple<string, SortOrder>("created_at", SortOrder.Ascending))
            .Column(u => u.Id, cm => cm.WithName("id").WithDbType(typeof(long)))
            .Column(u => u.GroupId, cm => cm.WithName("group_id").WithDbType(typeof(long)))
            .Column(u => u.UsersId, cm => cm.WithName("user_id").WithDbType(typeof(long)))
            .Column(u => u.CreatedAt, cm => cm.WithName("created_at").WithDbType(typeof(DateTime)))
            .Column(u => u.Status, cm => cm.WithName("status").WithDbType(typeof(bool)));
    }
}