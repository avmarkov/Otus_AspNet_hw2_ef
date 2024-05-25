using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Otus.Teaching.PromoCodeFactory.DataAccess.Repositories
{
    public class InMemoryRepository<T>
        : IRepository<T>
        where T : BaseEntity
    {
        protected List<T> Data { get; set; }

        public InMemoryRepository(IEnumerable<T> data)
        {
            Data = data.ToList();
        }

        public Task<IEnumerable<T>> GetAllAsync()
        {
            return Task.FromResult(Data.AsEnumerable());
        }

        public Task<T> GetByIdAsync(Guid id)
        {
            return Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
        }

        public Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
        {
            return Task.FromResult(Data.Where(e => ids.Contains(e.Id)).AsEnumerable());
        }

        public Task AddAsync(T item)
        {
            item.Id = Guid.NewGuid();
            Data.Add(item);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(T entity)
        {
            var data = Data.FirstOrDefault(x => x.Id == entity.Id);
            if (data == null)
                return null;

            int index = Data.IndexOf(data);
            if (index == -1)
            {
                return null;
            }

            Data[index] = entity;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            //T entity = Data.FirstOrDefault(x => x.Id == id);
            Data = Data.Where(x => x.Id != id).ToList();
            return Task.CompletedTask;
        }

        public Task<T> DeleteRangeAsync(IEnumerable<Guid> Ids)
        {
           throw new NotImplementedException();
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}