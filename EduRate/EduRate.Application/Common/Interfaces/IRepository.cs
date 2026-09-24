namespace EduRate.Application.Common.Interfaces
{
    /// <summary>
    /// Generic persistence abstraction. Application handlers depend on this (and the
    /// per-aggregate interfaces below) instead of EF Core's DbContext/DbSet directly.
    /// Query() exposes IQueryable so handlers can keep composing the same
    /// Include/Where/Select projections the original controllers used, without each
    /// repository needing a bespoke method per query shape.
    /// </summary>
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Query();

        /// <summary>Primary-key lookup (EF Core FindAsync semantics). Only valid for
        /// entities with a single int Id key - composite-key entities (e.g. TeacherCenter)
        /// should use Query() instead.</summary>
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
