using ICMD.Core.Authorization;
using ICMD.Core.DBModels;
using ICMD.Core.Shared;
using ICMD.EntityFrameworkCore.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Repository
{
    public class GenericRepository<DbContextType, T> : IGenericService<T> where DbContextType : DbContext where T : class
    {

        private readonly DbContextType _dbContext;
        private readonly DbSet<T> _table;
        private static List<string> excludeColumns = new List<string>() { "CreatedBy", "CreatedDate", "ModifiedBy", "ModifiedDate", "DeletedBy", "DeletedDate" };
        protected GenericRepository(DbContextType dbContext)
        {
            _dbContext = dbContext;
            _table = dbContext.Set<T>();
            //_assessor = accessor;
        }

        public T GetSingle(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return asNoTracking ? query.AsNoTracking().SingleOrDefault(predicate) : query.SingleOrDefault(predicate);
        }

        public T? GetFirstOrDefault(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return asNoTracking ? query.AsNoTracking().FirstOrDefault(predicate) : query.FirstOrDefault(predicate);
        }

        public virtual IEnumerable<T> GetAll(bool asNoTracking = false)
        {
            return asNoTracking ? _table.AsNoTracking() : _table.AsEnumerable();
        }
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (asNoTracking)
                return await query.AsNoTracking().SingleOrDefaultAsync(predicate);
            return await query.AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            if (asNoTracking)
                return await query.AsNoTracking().FirstOrDefaultAsync(predicate);
            return await query.FirstOrDefaultAsync(predicate);
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate, bool asNoTracking = false, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return asNoTracking ? query.AsNoTracking().Where(predicate) : query.Where(predicate);
        }
        public T GetById(object id)
        {
            return _table.Find(id);
        }
        public int GetCount(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return query.Count(predicate);
        }
        public virtual IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            return asNoTracking ? _table.AsNoTracking().Where(predicate) : _table.Where(predicate).AsEnumerable();
        }
        public virtual T Add(T entity, Guid? userId = null, bool isSave = true)
        {
            //Update default fields for addition and updation
            entity = UpdateDefaultFieldsForAddAndUpdate(entity, userId);
            _table.Add(entity);
            if (isSave) { Save(); }
            return entity;
        }
        public virtual async Task<T> AddAsync(T entity, Guid? userId = null, bool isSave = true)
        {
            entity = UpdateDefaultFieldsForAddAndUpdate(entity, userId);
            await _table.AddAsync(entity);
            LogAddedChanges(entity);
            if (isSave) { Save(); }
            return entity;
        }
        public virtual void AddRange(IEnumerable<T> entity, Guid? userId = null)
        {
            var addRange = entity.ToList();
            for (var i = 0; i < addRange.Count; i++)
            {
                //Update default fields for addition and updation
                addRange[i] = UpdateDefaultFieldsForAddAndUpdate(addRange[i], userId);
            }

            _table.AddRange(entity);
        }
        public virtual async Task AddRangeAsync(IEnumerable<T> entity, Guid? userId = null)
        {
            var addRange = entity.ToList();
            for (var i = 0; i < addRange.Count; i++)
            {
                //Update default fields for addition and updation
                addRange[i] = UpdateDefaultFieldsForAddAndUpdate(addRange[i], userId);
            }
            await _table.AddRangeAsync(addRange);
        }
        public virtual T Update(T entity, T oldEntity, Guid? userId = null, bool isSave = true, bool isDelete = false)
        {
            //Update default fields for addition and updation
            entity = UpdateDefaultFieldsForAddAndUpdate(entity, userId, true, isDelete);
            _table.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            LogModifiedChanges(entity, oldEntity, isDelete);
            if (isSave) { Save(); }
            return entity;
        }
        public virtual void Delete(T entity)
        {
            _table.Remove(entity);
            Save();
        }
        public void DeleteRange(IEnumerable<T> entity)
        {
            _table.RemoveRange(entity);
            Save();
        }
        public virtual void DeleteWhere(Expression<Func<T, bool>> predicate)
        {
            var entities = _table.Where(predicate);

            foreach (var entity in entities)
            {
                _dbContext.Entry<T>(entity).State = EntityState.Deleted;
            }
            Save();
        }
        public virtual void Save()
        {
            _dbContext.SaveChanges();
        }
        public virtual void Detach(T entity)
        {
            EntityEntry dbEntityEntry = _dbContext.Entry<T>(entity);
            switch (dbEntityEntry.State)
            {
                case EntityState.Modified:
                    dbEntityEntry.State = EntityState.Unchanged;
                    break;
                case EntityState.Added:
                    dbEntityEntry.State = EntityState.Detached;
                    break;
                case EntityState.Deleted:
                    dbEntityEntry.Reload();
                    break;
            }
        }
        public virtual async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<IDbContextTransaction> BeginTransaction()
        {
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public virtual async Task RollbackTransaction(IDbContextTransaction transaction)
        {
            await transaction.RollbackAsync();
        }

        public T UpdateDefaultFieldsForAddAndUpdate(T entity, Guid? userId, bool isEdit = false, bool isDelete = false)
        {
            if (!isDelete)
            {
                //Add createdBy and CreadetDate
                if (!isEdit)
                {
                    if (entity.GetType().GetProperty("CreatedBy") != null)
                    {
                        entity.GetType().GetProperty("CreatedBy")?.SetValue(entity, userId);
                    }
                    if (entity.GetType().GetProperty("CreatedDate") != null)
                    {
                        entity.GetType().GetProperty("CreatedDate")?.SetValue(entity, DateTime.UtcNow);
                    }
                }
                //Add updatedby and updatedDate
                else
                {
                    if (entity.GetType().GetProperty("ModifiedBy") != null)
                    {
                        entity.GetType().GetProperty("ModifiedBy")?.SetValue(entity, userId);
                    }
                    if (entity.GetType().GetProperty("ModifiedDate") != null)
                    {
                        entity.GetType().GetProperty("ModifiedDate")?.SetValue(entity, DateTime.UtcNow);
                    }
                }
            }
            else
            {
                if (entity.GetType().GetProperty("DeletedBy") != null)
                {
                    entity.GetType().GetProperty("DeletedBy")?.SetValue(entity, userId);
                }
                if (entity.GetType().GetProperty("DeletedDate") != null)
                {
                    entity.GetType().GetProperty("DeletedDate")?.SetValue(entity, DateTime.UtcNow);
                }
            }

            return entity;
        }

        public void LogAddedChanges(T entity)
        {
            var addedEntities = _dbContext.ChangeTracker.Entries()
          .Where(e => e.State == EntityState.Added)
          .ToList();

            foreach (var entry in addedEntities)
            {
                // Ignore new AuditLogItems
                if (entity.GetType() == typeof(ChangeLog))
                    continue;

                Guid id = Guid.Empty;
                var entityName = "";
                var newValues = new StringBuilder();

                var entityType = _dbContext.Model.FindEntityType(entry.Entity.GetType());

                entityName = entry.Entity.GetType().Name;

                // Store added objects changes 
                var currentValues = entry.CurrentValues;

                foreach (var property in currentValues.Properties)
                {
                    if (!excludeColumns.Contains(property.Name))
                    {
                        newValues.Append(property.Name).Append(": ");
                        newValues.Append(currentValues[property]).Append(", ");
                    }
                }

                newValues.Remove(newValues.Length - 2, 2);
                AddLog(1, entity, currentValues, newValues, entityName);
            }

        }

        public void LogModifiedChanges(T entity, T oldEntity, bool isDeleted = false)
        {
            var modifiedEntities = _dbContext.ChangeTracker.Entries()
                   .Where(e => e.State == EntityState.Modified)
                   .ToList();
            var oldEntry = oldEntity != null ? _dbContext.Entry(oldEntity) : null;
            foreach (var entry in modifiedEntities)
            {
                var newValues = new StringBuilder();
                var oldValues = new StringBuilder();
                var currentValues = entry.CurrentValues;
                var originalValues = entry.OriginalValues;

                Guid id = Guid.Empty;

                // Manually mark properties as modified
                if (!isDeleted)
                {
                    foreach (var propName in currentValues.Properties)
                    {
                        if (!excludeColumns.Contains(propName.Name))
                        {
                            var newValue = currentValues[propName];
                            var oldValue = oldEntry?.CurrentValues[propName];

                            if (!object.Equals(newValue, oldValue))
                            {
                                newValues.Append(propName.Name).Append(": ").Append(newValue).Append(", ");
                                oldValues.Append(propName.Name).Append(": ").Append(oldValue).Append(", ");

                                // Mark the property as modified
                                entry.Property(propName).IsModified = true;
                            }
                        }

                    }
                    if (newValues.ToString() != "")
                    {
                        newValues.Remove(newValues.Length - 2, 2);
                        oldValues.Remove(oldValues.Length - 2, 2);

                        AddLog(0, entity, currentValues, newValues, entry.Entity.GetType().Name, oldValues);
                    }
                }
                else
                {
                    foreach (var property in currentValues.Properties)
                    {
                        if (!excludeColumns.Contains(property.Name))
                        {
                            oldValues.Append(property.Name).Append(": ");
                            oldValues.Append(currentValues[property]).Append(", ");
                        }
                    }
                    AddLog(-1, entity, currentValues, newValues, entry.Entity.GetType().Name, oldValues);
                }

            }
        }

        private void AddLog(int isAdd, T entity, PropertyValues currentValues, StringBuilder? newValues, string entityName, StringBuilder? originalValues = null)
        {
            Guid id = currentValues != null && !string.IsNullOrEmpty(currentValues["Id"]?.ToString()) ? new Guid(currentValues["Id"].ToString()) : Guid.Empty;
            Guid userId = currentValues != null && !string.IsNullOrEmpty(currentValues["CreatedBy"]?.ToString()) ? new Guid(currentValues["CreatedBy"].ToString() ?? null) : Guid.Empty;
            Guid? projectId = GetProjectId(userId);
            var changeLogItem = new ChangeLog()
            {
                Context = entity.GetType().Name,
                ContextId = id,
                EntityName = entityName,
                EntityId = id,
                Status = isAdd == -1 ? EntityState.Deleted.ToString() : (isAdd == 1 ? EntityState.Added.ToString() : EntityState.Modified.ToString()),
                NewValues = newValues?.ToString() ?? "",
                OriginalValues = originalValues?.ToString() ?? "N/A",
                ProjectId = projectId,
                CreatedBy = userId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            _dbContext.Add(changeLogItem);
            _dbContext.SaveChanges();
        }

        private Guid? GetProjectId(Guid userId)
        {
            ICMDUser? userInfo = _dbContext.Set<ICMDUser>().FirstOrDefault(a => a.Id == userId);
            return userInfo?.ProjectId ?? null;
        }
    }
}
