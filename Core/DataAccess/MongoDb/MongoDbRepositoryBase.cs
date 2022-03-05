using System.Linq.Expressions;
using Core.DataAccess.MongoDb.Configurations;
using Core.Entities;
using Core.Utilities.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Core.DataAccess.MongoDb
{
    public abstract class MongoDbRepositoryBase<T> : IRepository<T>  where T : class, IEntity
    {
        private readonly IMongoCollection<T> _collection;

        protected MongoDbRepositoryBase(string collectionName)
        {
            var configuration = ServiceTool.ServiceProvider.GetService<IConfiguration>();
            var mongoConnectionSetting = 
                configuration.GetSection("MongoDbSettings").Get<MongoConnectionSettings>();
            var url =
                $"mongodb://{mongoConnectionSetting.UserName}:{mongoConnectionSetting.Password}@{mongoConnectionSetting.Host}:{mongoConnectionSetting.Port}/?readPreference=primary&appname=MongoDB%20Compass&ssl=false";
            var client = new MongoClient(url);
            var database = client.GetDatabase(mongoConnectionSetting.DatabaseName);
            _collection = database.GetCollection<T>(collectionName);
        }

        public virtual T GetById(long id)
        {
            return _collection.Find(x => x.Id == id).FirstOrDefault();
        }

        public virtual async Task<T> GetByIdAsync(long id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public virtual void Add(T entity)
        {
            var id = _collection.AsQueryable().OrderByDescending(x => x.Id).First().Id;
            entity.Id = id + 1;
            var options = new InsertOneOptions {BypassDocumentValidation = false};
            _collection.InsertOne(entity, options);
        }

        public virtual async Task AddAsync(T entity)
        {
            var id = _collection.AsQueryable().OrderByDescending(x => x.Id).First().Id;
            entity.Id = id + 1;
            var options = new InsertOneOptions {BypassDocumentValidation = false};
            await _collection.InsertOneAsync(entity, options);
        }

        public virtual IQueryable<T> GetList(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? _collection.AsQueryable()
                : _collection.AsQueryable().Where(predicate);
        }

        public virtual async Task<IQueryable<T>> GetListAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await Task.Run(() =>
            {
                return predicate == null
                    ? _collection.AsQueryable()
                    : _collection.AsQueryable().Where(predicate);
            });
        }

        public virtual void Update(T record)
        {
            _collection.FindOneAndReplace(x => x.Id == record.Id, record);
        }

        public virtual async Task UpdateAsync(T record)
        {
            await _collection.FindOneAndReplaceAsync(x => x.Id == record.Id, record);
        }

        public bool Any(Expression<Func<T, bool>> predicate = null)
        {
            var data = predicate == null
                ? _collection.AsQueryable()
                : _collection.AsQueryable().Where(predicate);

            return data.FirstOrDefault() != null;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await Task.Run(() =>
            {
                var data = predicate == null
                    ? _collection.AsQueryable()
                    : _collection.AsQueryable().Where(predicate);

                return data.FirstOrDefault() != null;
            });
        }

        public long GetCount(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? _collection.AsQueryable().Count()
                : _collection.AsQueryable().Where(predicate).Count();
        }

        public async Task<long> GetCountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return await Task.Run(() => predicate == null
                ? _collection.AsQueryable().Count()
                : _collection.AsQueryable().Where(predicate).Count());
        }
    }
}