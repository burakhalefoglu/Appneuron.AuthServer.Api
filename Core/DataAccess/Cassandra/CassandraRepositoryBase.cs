﻿using System.Linq.Expressions;
using Cassandra;
using Cassandra.Data.Linq;
using Cassandra.Mapping;
using Core.DataAccess.Cassandra.Configurations;
using Core.Entities;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DataAccess.Cassandra
{
    // https://github.com/datastax/csharp-driver
    public class CassandraRepositoryBase<T>
        : IRepository<T>
        where T : class, IEntity, new()
    {
        private readonly Table<T> _table;

        protected CassandraRepositoryBase(MappingConfiguration mappingConfiguration)
        {
            var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
            var cassandraConnectionSettings = 
                    configuration.GetSection("CassandraConnectionSettings").Get<CassandraConnectionSettings>();
            var cluster = Cluster.Builder()
                .AddContactPoints(cassandraConnectionSettings.Host)
                .WithCredentials(cassandraConnectionSettings.UserName, cassandraConnectionSettings.Password)
                .WithApplicationName("AuthServer")
                .WithCompression(CompressionType.Snappy)
                .Build();
            var session = cluster.Connect();
            session.CreateKeyspaceIfNotExists(cassandraConnectionSettings.Keyspace);
            _table = new Table<T>(session, mappingConfiguration);
            _table.CreateIfNotExists();
        }

        public IQueryable<T> GetList(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? _table.Execute().AsQueryable()
                : _table.Where(predicate)
                    .Execute().AsQueryable();
        }

        public T GetById(long id)
        {
            return _table
                .FirstOrDefault(u => u.Id == id).Execute();
        }

        public long GetCount(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? _table.Count().Execute()
                : _table.Where(predicate)
                    .Count().Execute();
        }

        public async Task<long> GetCountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await Task.Run(() => predicate == null
                ? _table.Count().Execute()
                : _table.Where(predicate)
                    .Count().Execute());
        }

        public async Task<IQueryable<T>> GetListAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await Task.Run(() => predicate == null
                ? _table.Execute().AsQueryable()
                : _table.Where(predicate)
                    .Execute().AsQueryable());
        }

        public async Task<T> GetByIdAsync(long id)
        {
            return await Task.Run(() =>
            {
                return _table
                    .FirstOrDefault(u => u.Id == id).Execute();
            });
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return (await Task.Run(() => _table.Where(predicate).FirstOrDefault().Execute()))!;
        }

        public bool Any(Expression<Func<T, bool>> predicate = null)
        {
            var data = predicate == null
                ? _table.FirstOrDefault().Execute()
                : _table.Where(predicate).FirstOrDefault().Execute();

            return data != null;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await Task.Run(() =>
            {
                var data = predicate == null
                    ? _table.FirstOrDefault().Execute()
                    : _table.Where(predicate).FirstOrDefault().Execute();

                return data != null;
            });
        }

        public void Add(T entity)
        {
            var filter = _table.Execute().MaxBy(e => e.Id);
            var id = filter?.Id ?? 0;
            entity.Id = id + 1;
            _table.Insert(entity).Execute();
        }

        public async Task AddAsync(T entity)
        {
            await Task.Run(() =>
            {
                var filter = _table.Execute().MaxBy(e => e.Id);
                var id = filter?.Id ?? 0;
                entity.Id = id + 1;
                _table.Insert(entity).Execute();
            });
        }

        public async Task UpdateAsync(T record, Expression<Func<T, bool>> predicate)
        {
            await Task.Run(() =>
            {
                _table.Where(predicate)
                    .Select(u => record)
                    .Update()
                    .Execute();
            });
        }
    }
}