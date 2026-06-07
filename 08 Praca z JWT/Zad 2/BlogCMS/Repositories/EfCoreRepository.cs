using BlogCMS.Data;
using BlogCMS.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogCMS.Repositories
{
    public class EfCoreRepository<T> : IRepository<T> where T : class
    {
        private readonly BlogDbContext _context;
        private readonly DbSet<T> _entities;

        public EfCoreRepository(BlogDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _entities.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task<int> AddAsync(T entity)
        {
            await _entities.AddAsync(entity);
            await _context.SaveChangesAsync();

            return GetEntityId(entity);
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            var id = GetEntityId(entity);
            var existingEntity = await GetByIdAsync(id);
            if (existingEntity == null)
            {
                return false;
            }

            _context.Entry(existingEntity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }

            _entities.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        private static int GetEntityId(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException($"Entity {typeof(T).Name} must have an Id property.");
            }

            var value = idProperty.GetValue(entity);
            if (value is not int id)
            {
                throw new InvalidOperationException($"Entity {typeof(T).Name} Id property must be an int.");
            }

            return id;
        }
    }
}
