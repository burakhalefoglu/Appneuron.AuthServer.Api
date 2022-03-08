using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.DataAccess.EntityFramework
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public class EfEntityRepositoryBase<TEntity, TContext>
        : IRepository<TEntity>
        where TEntity : class, IEntity, new()
        where TContext : DbContext
    {
        protected readonly TContext Context;

        public EfEntityRepositoryBase(TContext context)
        {
            Context = context;
        }

        public void Add(TEntity entity)
        {
            Context.Add(entity);
            Context.SaveChanges();
        }
        
        public async Task AddAsync(TEntity entity)
        {
            await Context.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            Context.Update(entity);
            Context.SaveChanges();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            await Task.Run(() =>
            {
                entity.Status = false;
                Context.Update(entity);
                Context.SaveChanges();
                
            });
        }

        public void Delete(TEntity entity)
        {
            
            entity.Status = false;
            Context.Update(entity);
            Context.SaveChanges();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            await Task.Run(() => Context.Update(entity));
            await Context.SaveChangesAsync();
        }

        public TEntity Get(Expression<Func<TEntity, bool>> expression)
        {
            return Context.Set<TEntity>().FirstOrDefault(expression);
        }

        public async Task<TEntity> GetByIdAsync(long id)
        {
            return await Context.Set<TEntity>().AsQueryable().FirstOrDefaultAsync(x=> x.Id == id);
        }
        
        public TEntity GetById(long id)
        {
            return Context.Set<TEntity>().AsQueryable().FirstOrDefault(x=> x.Id == id);
        }
        
        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await Context.Set<TEntity>().AsQueryable().FirstOrDefaultAsync(expression);
        }
        
        public bool Any(Expression<Func<TEntity, bool>> predicate = null)
        {
            var data = predicate == null
                ? Context.Set<TEntity>().FirstOrDefault()
                : Context.Set<TEntity>().Where(predicate).FirstOrDefault();
            return data != null;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            return await Task.Run(() =>
            {
                var data = predicate == null
                    ? Context.Set<TEntity>().FirstOrDefault()
                    : Context.Set<TEntity>().Where(predicate).FirstOrDefault();

                return data != null;
            });
        }

        public IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> expression = null)
        {
            return expression == null
                ? Context.Set<TEntity>().AsNoTracking()
                : Context.Set<TEntity>().Where(expression).AsNoTracking();
        }



        public async Task<IQueryable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> expression = null)
        {
            return await Task.Run(() => expression == null
                ? Context.Set<TEntity>()
                : Context.Set<TEntity>().Where(expression));
        }

        public int SaveChanges()
        {
            return Context.SaveChanges();
        }

        public Task<int> SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }

        public IQueryable<TEntity> Query()
        {
            return Context.Set<TEntity>();
        }

        public Task<int> Execute(FormattableString interpolatedQueryString)
        {
            return Context.Database.ExecuteSqlInterpolatedAsync(interpolatedQueryString);
        }

        /// <summary>
        ///     Transactional operations is prohibited when working with InMemoryDb!
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="action"></param>
        /// <param name="successAction"></param>
        /// <param name="exceptionAction"></param>
        /// <returns></returns>
        public TResult InTransaction<TResult>(Func<TResult> action, Action successAction = null,
            Action<Exception> exceptionAction = null)
        {
            var result = default(TResult);
            try
            {
                if (Context.Database.ProviderName.EndsWith("InMemory"))
                {
                    result = action();
                    SaveChanges();
                }
                else
                {
                    using (var tx = Context.Database.BeginTransaction())
                    {
                        try
                        {
                            result = action();
                            SaveChanges();
                            tx.Commit();
                        }
                        catch (Exception)
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                successAction?.Invoke();
            }
            catch (Exception ex)
            {
                if (exceptionAction == null)
                    throw;
                exceptionAction(ex);
            }

            return result;
        }

        public async Task<long> GetCountAsync(Expression<Func<TEntity, bool>> expression = null)
        {
            if (expression == null)
                return await Context.Set<TEntity>().CountAsync();
            return await Context.Set<TEntity>().CountAsync(expression);
        }

        public long GetCount(Expression<Func<TEntity, bool>> expression = null)
        {
            if (expression == null)
                return Context.Set<TEntity>().Count();
            return Context.Set<TEntity>().Count(expression);
        }
    }
}