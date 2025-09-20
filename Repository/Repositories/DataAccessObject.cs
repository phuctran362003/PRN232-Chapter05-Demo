using Microsoft.EntityFrameworkCore;
using Repository.Entities;

namespace Repository.Repositories
{
    public class DataAccessObject<T> where T : class
    {
        protected WatercolorsPainting2024DbContext _context;

        // Default constructor, initializes the DbContext if null
        public DataAccessObject()
        {
            _context ??= new WatercolorsPainting2024DbContext();
        }

        // Constructor that accepts a DbContext
        public DataAccessObject(WatercolorsPainting2024DbContext context)
        {
            _context = context;
        }

        // Synchronous method to retrieve all entities
        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        // Asynchronous method to retrieve all entities
        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // Synchronous method to create a new entity
        public void Create(T entity)
        {
            _context.Add(entity);
            _context.SaveChanges();
        }

        // Asynchronous method to create a new entity
        public async Task<int> CreateAsync(T entity)
        {
            _context.Add(entity);
            return await _context.SaveChangesAsync();
        }

        // Synchronous method to update an entity
        public void Update(T entity)
        {
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
            _context.SaveChanges();
        }

        // Asynchronous method to update an entity
        public async Task<int> UpdateAsync(T entity)
        {
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        // Synchronous method to remove an entity
        public bool Remove(T entity)
        {
            _context.Remove(entity);
            _context.SaveChanges();
            return true;
        }

        // Asynchronous method to remove an entity
        public async Task<bool> RemoveAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // Retrieve entity by integer ID (synchronously)
        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        // Retrieve entity by integer ID (asynchronously)
        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // Retrieve entity by string ID (synchronously)
        public T GetById(string code)
        {
            return _context.Set<T>().Find(code);
        }

        // Retrieve entity by string ID (asynchronously)
        public async Task<T> GetByIdAsync(string code)
        {
            return await _context.Set<T>().FindAsync(code);
        }

        // Retrieve entity by Guid (synchronously)
        public T GetById(Guid code)
        {
            return _context.Set<T>().Find(code);
        }

        // Retrieve entity by Guid (asynchronously)
        public async Task<T> GetByIdAsync(Guid code)
        {
            return await _context.Set<T>().FindAsync(code);
        }

        #region Separating assign entity and save operations

        // Prepare an entity for creation (no save)
        public void PrepareCreate(T entity)
        {
            _context.Add(entity);
        }

        // Prepare an entity for update (no save)
        public void PrepareUpdate(T entity)
        {
            var tracker = _context.Attach(entity);
            tracker.State = EntityState.Modified;
        }

        // Prepare an entity for removal (no save)
        public void PrepareRemove(T entity)
        {
            _context.Remove(entity);
        }

        // Save changes synchronously
        public int Save()
        {
            return _context.SaveChanges();
        }

        // Save changes asynchronously
        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
    #endregion

}
