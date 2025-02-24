using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore.Storage;

namespace ICMD.Core.Shared
{
    public interface IGenericService<T> where T : class
    {
        T GetSingle(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties);
        T? GetFirstOrDefault(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties);
        IEnumerable<T> GetAll(bool asNoTracking = false);
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includePropertie);
        T GetById(object id);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate, bool asNoTracking = false);
        int GetCount(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);
        T Add(T entity, Guid? userId = null, bool isSave = true);
        Task<T> AddAsync(T entity, Guid? userId = null, bool isSave = true);
        void AddRange(IEnumerable<T> entity, Guid? userId = null);
        Task AddRangeAsync(IEnumerable<T> entity, Guid? userId = null);
        T Update(T entity, T oldEntity, Guid? userId = null, bool isSave = true, bool isDelete = false);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entity);
        void Save();
        void Detach(T entity);
        Task SaveAsync();
        Task<IDbContextTransaction> BeginTransaction();
        Task RollbackTransaction(IDbContextTransaction transaction);
    }
}
