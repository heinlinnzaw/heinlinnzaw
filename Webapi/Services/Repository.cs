using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Webapi.Services
{
    public interface IRepository<T>
    {
        IQueryable<T> GetAll();
        IQueryable<T> Query(Expression<Func<T, bool>> filter);
        void Insert(T entity);
        T InsertReturn(T entity);
        Task<T> InsertReturnAsync(T entity);
        void Delete(T entity);
        T Update(T OldEntity, T NewEntity);
        T UpdatePartial(T OldEntity, T NewEntity, params Expression<Func<T, object>>[] propertiesToUpdate);
        Task<T> UpdateCompleteAsync(T OldEntity, T NewEntity);
    }
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        public Repository(DbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public IQueryable<T> GetAll()
        {
            try
            {
                return _dbContext.Set<T>().AsQueryable();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public IQueryable<T> Query(Expression<Func<T, bool>> filter)
        {
            return _dbContext.Set<T>().Where(filter).AsQueryable();
        }
        public void Insert(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }
        public T InsertReturn(T entity)
        {
            T newEntity = _dbContext.Set<T>().Add(entity).Entity;
            return newEntity;
        }
        public async Task<T> InsertReturnAsync(T entity)
        {
            T newEntity = _dbContext.Set<T>().Add(entity).Entity;
            var result = await _dbContext.SaveChangesAsync();
            if (result > 0)
            {
                return newEntity;
            }
            else
            {
                return null;
            }
        }
        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }
        public T Update(T OldEntity, T NewEntity)
        {
            try
            {
                _dbContext.Entry<T>(OldEntity).State = EntityState.Detached;
                _dbContext.Set<T>().Attach(NewEntity);
                _dbContext.Entry(NewEntity).State = EntityState.Modified;
                return NewEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public T UpdatePartial(T OldEntity,T NewEntity, params Expression<Func<T, object>>[] propertiesToUpdate)
        {
            _dbContext.Entry<T>(OldEntity).State = EntityState.Detached;
            _dbContext.Set<T>().Attach(NewEntity);
            foreach (var p in propertiesToUpdate)
            {
                _dbContext.Entry(NewEntity).Property(p).IsModified = true;
            }
            return NewEntity;
        }
        public async Task<T> UpdateCompleteAsync(T OldEntity,T NewEntity)
        {
            _dbContext.Entry(OldEntity).State = EntityState.Detached;
            _dbContext.Set<T>().Attach(NewEntity);
            _dbContext.Entry(NewEntity).State = EntityState.Modified;
            var result = await _dbContext.SaveChangesAsync();
            if (result > 0)
            {
                return NewEntity;
            }
            else
            {
                return null;
            }
        }
    }
}
