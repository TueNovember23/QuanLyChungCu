using Microsoft.EntityFrameworkCore;
using Repositories.Repositories.Base;
using Repositories.Interfaces;
using System.Linq.Expressions;

namespace Repositories.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext dbContext)
        {
            _context = dbContext;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> Entities => _context.Set<T>();

        public void Delete(object id)
        {
            T entity = _dbSet.Find(id)!;
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task DeleteAsync(object id)
        {
            T entity = await _dbSet.FindAsync(id) ?? throw new Exception();
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<IQueryable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await Task.FromResult(_dbSet.Where(predicate));
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<List<T>> FindListAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public IEnumerable<T> Get(int index, int pageSize)
        {
            return [.. _dbSet.Skip(index * pageSize).Take(pageSize)];
        }


        public List<T> GetAll()
        {
            return [.. _context.Set<T>()];
        }

        public async Task<IQueryable<T>> GetAllQueryableAsync()
        {
            return await Task.FromResult(_dbSet.AsQueryable());
        }

        public async Task<IEnumerable<T>> GetAsync(int index, int pageSize)
        {
            return await _dbSet.Skip(index * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetBlocAsync(int pageIndex, int pageSize)
        {
            return await Entities
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public T GetById(object id)
        {

            return _dbSet.Find(id)!;
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Insert(T obj)
        {
            _dbSet.Add(obj);
        }

        public async Task InsertAsync(T obj)
        {
            await _dbSet.AddAsync(obj);
        }

        public void InsertRange(List<T> obj)
        {
            _dbSet.AddRange(obj);
        }
        public async Task InsertCollection(ICollection<T> collection)
        {
            await _dbSet.AddRangeAsync(collection);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(T obj)
        {
            _dbSet.Attach(obj);
            _dbSet.Entry(obj).State = EntityState.Modified;
        }

        public async Task UpdateAsync(T obj)
        {
            _dbSet.Attach(obj);
            _dbSet.Entry(obj).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public IQueryable<T> Find_Sync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsQueryable();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public IQueryable<T> FindAll()
        {
            return _dbSet; 
        }
    }
}
