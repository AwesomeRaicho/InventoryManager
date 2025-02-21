using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InventoryManager.Core.Interfaces;

namespace InventoryManager.Infrastructure.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly EntityDbContext _dbContext;

        public Repository(EntityDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        //  Create
        public async Task<T> Create(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");

            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        //  Delete
        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "Id cannot be null.");

            var entity = await _dbContext.Set<T>().FindAsync(Guid.Parse(id));
            if (entity == null)
                throw new KeyNotFoundException($"Entity with ID {id} not found.");

            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        //  Find Single Entity by Condition
        public async Task<T?> Find(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null.");

            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        //  FindAll with Paging (Efficient Pagination)
        public async Task<IEnumerable<T>> FindAll(IQueryable<T> query, int? pageIndex, int? pageSize)
        {
            if (query == null)
                throw new ArgumentException("[CE] Query cannot be null.");

            int page = pageIndex ?? 1;
            int size = pageSize ?? 10;

            return await query.Skip((page - 1) * size).Take(size).ToListAsync();
        }

        //  Get by ID
        public async Task<T?> GetEntityById(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), "ID cannot be null.");

            return await _dbContext.Set<T>().FindAsync(Guid.Parse(id));
        }

        //  Update
        public async Task<T> Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");

            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        //  Get IQueryable (Best for Business Layer Filtering)
        public IQueryable<T> GetQueryable()
        {
            return _dbContext.Set<T>();
        }

        //  Find with Includes (Eager Loading)
        public async Task<T?> Find(Expression<Func<T, bool>> predicate, params Func<IQueryable<T>, IQueryable<T>>[] includes)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            var query = _dbContext.Set<T>().Where(predicate);
            foreach (var include in includes)
            {
                query = include(query);
            }
            return await query.FirstOrDefaultAsync();
        }

        //  Efficient IsUnique Check
        public async Task<bool> IsUnique(Expression<Func<T, bool>> predicate)
        {
            return !await _dbContext.Set<T>().AnyAsync(predicate);
        }

        //  Add Multiple Entities
        public async Task AddRange(List<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentException("Entities cannot be null or empty.", nameof(entities));

            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        //Remove Multiple Entities
        public async Task RemoveRange(List<T> entities)
        {
            if (entities == null || !entities.Any())
                throw new ArgumentException("Entities cannot be null or empty.", nameof(entities));

            _dbContext.Set<T>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }
    }
}
