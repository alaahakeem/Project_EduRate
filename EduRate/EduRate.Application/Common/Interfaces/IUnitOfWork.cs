namespace EduRate.Application.Common.Interfaces
{
    /// <summary>
    /// Commits changes made through any repository in the current scope. All repositories
    /// and the UnitOfWork share the same scoped DbContext instance, so a single
    /// SaveChangesAsync() call persists everything atomically - exactly like the original
    /// controllers' single "_context.SaveChangesAsync()" call did.
    /// </summary>
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
