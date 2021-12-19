using Microsoft.EntityFrameworkCore;
using Shared.Options;
using System.Linq;

namespace Domain.Shared.DatabaseContext
{
    public interface IReadOnlyDataContext
    {
        TEntity Get<TEntity>(long id) where TEntity : BaseEntity;
        IQueryable<TEntity> GetQuery<TEntity>() where TEntity : BaseEntity;
        IQueryable<TEntity> GetQueryWithDeleted<TEntity>() where TEntity : BaseEntity;
        TEntity GetWithDeleted<TEntity>(long id) where TEntity : BaseEntity;
    }

    public class ReadOnlyDataContext : IReadOnlyDataContext
    {
        private readonly DataContext _context;

        public ReadOnlyDataContext(DataContext context, QueryConnectionString queryConnectionString)
        {
            _context = context;
            _context.Database.SetConnectionString(queryConnectionString.ConnectionString);
        }

        public IQueryable<TEntity> GetQuery<TEntity>() where TEntity : BaseEntity
        {
            return _context.Set<TEntity>().Where(x => !x.IsDeleted).AsNoTracking().AsQueryable();
        }

        public IQueryable<TEntity> GetQueryWithDeleted<TEntity>() where TEntity : BaseEntity
        {
            return _context.Set<TEntity>().AsNoTracking();
        }

        public TEntity Get<TEntity>(long id) where TEntity : BaseEntity
        {
            return _context.Set<TEntity>().SingleOrDefault(x => x.Id == id && !x.IsDeleted);
        }

        public TEntity GetWithDeleted<TEntity>(long id) where TEntity : BaseEntity
        {
            return _context.Set<TEntity>().SingleOrDefault(x => x.Id == id);
        }
    }
}