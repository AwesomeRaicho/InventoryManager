using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using InventoryManager.Core.Interfaces;

namespace InventoryManager.Infrastructure.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly EntityDbContext _dbContext;

        public Repository(EntityDbContext myDbContext)
        {
            _dbContext = myDbContext;
        }

        public async Task<T> Create(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null when creating.");
            }

            try
            {
                await _dbContext.Set<T>().AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while creating entity.", ex);
            }
        }


        public async Task<bool> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), "Id cannot be null when deleteing.");
            }

            var entity = await _dbContext.Set<T>().FindAsync(Guid.Parse(id));

            if (entity == null)
            {
                throw new InvalidOperationException("An error occurred when trying to delete entity.", new KeyNotFoundException("Data base returned null, entity does not exist."));
            }

            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<T?> Find(Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
            {
                throw new InvalidOperationException("Predicate parameter cannot be null");
            }

            try
            {
                return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while searching the requested data.", ex);
            }
        }
        public async Task<IEnumerable<T>> FindAll(IQueryable<T> query, int? pageIndex, int? pageSize)
        {
            if (query == null)
            {
                throw new Exception("[CE] Query to DB cannot be null.");
            }



            int pageindex = pageIndex == null || pageIndex == 0 ? 1 : (int)pageIndex;
            int pagesize = pageSize ?? 1;
            var returns = await query.Skip((pageindex - 1) * pagesize).Take(pagesize)
                .ToListAsync();
            return returns;
        }

        public async Task<T?> GetEntityById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id), "ID cannot be null when getting entity");
            }


            try
            {

                return await _dbContext.Set<T>().FindAsync(Guid.Parse(id));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while getting entity", ex);
            }


        }

        public async Task<bool> Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity being updated cannot be null");
            }

            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync(); 

            return true;
        }

        public IQueryable<T> GetQueryable()
        {
            return _dbContext.Set<T>();
        }

        public Task<T?> Find(Expression<Func<T, bool>> predicate, params Func<IQueryable<T>, IQueryable<T>>[] includes)
        {
            var query = _dbContext.Set<T>().Where(predicate);

            foreach(var include in includes)
            {
                query = include(query);
            }

            return query.FirstOrDefaultAsync();
        }
    }

}
