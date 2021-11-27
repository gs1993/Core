using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApi.Entities.Shared;

namespace WebApi.Helpers
{
    public interface IReadonlyDataContext
    {
        TEntity Get<TEntity>(long id) where TEntity : BaseEntity;
        IQueryable<TEntity> GetQuery<TEntity>() where TEntity : BaseEntity;
        IQueryable<TEntity> GetQueryWithDeleted<TEntity>() where TEntity : BaseEntity;
        TEntity GetWithDeleted<TEntity>(long id) where TEntity : BaseEntity;
    }

    public class ReadonlyDataContext : IReadonlyDataContext
    {
        private readonly DataContext _context;

        public ReadonlyDataContext(DataContext context)
        {
            _context = context; //TODO: change connection string for queries
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