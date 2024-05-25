using Microsoft.EntityFrameworkCore;
using Otus.Teaching.PromoCodeFactory.Core.Abstractions.Repositories;
using Otus.Teaching.PromoCodeFactory.Core.Domain;
using Otus.Teaching.PromoCodeFactory.DataAccess.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Otus.Teaching.PromoCodeFactory.DataAccess.Repositories
{
    public class EfRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DataBaseContext _datacontext;
        protected DbSet<T> Data;

        public EfRepository(DataBaseContext datacontext)
        {
            _datacontext = datacontext;
            Data = _datacontext.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Data.ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await Data.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<T>> GetRangeByIdsAsync(List<Guid> ids)
        {
            return await Data.Where(e => ids.Contains(e.Id)).ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await Data.AddAsync(entity);
            await _datacontext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            Data.Update(entity);
            await _datacontext.SaveChangesAsync();
            
        }

        public async Task DeleteAsync(Guid Id)
        {
            var entity = await GetByIdAsync(Id);

            Data.Remove(entity);
            await _datacontext.SaveChangesAsync();           
        }

        public async Task<T> DeleteRangeAsync(IEnumerable<Guid> Ids)
        {
            var entities = await Data.FindAsync(Ids);
            Data.RemoveRange(entities);

            await _datacontext.SaveChangesAsync();
            return entities;
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await Data.FirstOrDefaultAsync(predicate);
        }
    }
}