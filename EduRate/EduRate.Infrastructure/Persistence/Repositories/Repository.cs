using EduRate.Application.Common.Interfaces;
using EduRate.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EduRate.Infrastructure.Persistence.Repositories
{
    /// <summary>Generic EF Core-backed implementation shared by every specific repository below.</summary>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly AppDbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(AppDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public IQueryable<TEntity> Query() => DbSet.AsQueryable();

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
            => await DbSet.FindAsync(new object[] { id }, cancellationToken);

        public void Add(TEntity entity) => DbSet.Add(entity);

        public void AddRange(IEnumerable<TEntity> entities) => DbSet.AddRange(entities);

        public void Update(TEntity entity) => DbSet.Update(entity);

        public void Remove(TEntity entity) => DbSet.Remove(entity);

        public void RemoveRange(IEnumerable<TEntity> entities) => DbSet.RemoveRange(entities);
    }
}
