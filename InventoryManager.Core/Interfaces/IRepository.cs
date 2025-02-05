using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManager.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        public IQueryable<T> GetQueryable();
        public Task<T> Create(T entity);
        public Task<T?> GetEntityById(string id);
        public Task<T?> Find(Expression<Func<T, bool>> predicate);
        public Task<T?> Find(Expression<Func<T, bool>> predicate, params Func<IQueryable<T>, IQueryable<T>>[] includes);
        public Task<IEnumerable<T>> FindAll(IQueryable<T> query, int? pageIndex, int? pageSize);
        public Task<bool> Update(T entity);
        public Task<bool> Delete(string id);

    }
}
