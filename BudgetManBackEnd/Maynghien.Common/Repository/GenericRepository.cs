

using MayNghien.Common.Models.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Maynghien.Common.Repository
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity, TContext>
        where TEntity : BaseEntity where TContext : IdentityDbContext
    {
        #region Properties
        public TContext _context;
        private bool disposed = false;

        public TContext DbContext
        {
            get
            {
                return _context;
            }
        }
        #endregion

        #region Constructor
        public GenericRepository(TContext unitOfWork)
        {
            _context = unitOfWork;
        }


        #endregion

        #region Method
        public virtual void Add(TEntity item)
        {
            if (item != null)
            {
                item.CreatedOn = DateTime.UtcNow;
                if (item.CreatedBy == null)
                {
                    item.CreatedBy = "";
                }
                _context.Add(item);
                _context.SaveChanges();
            }
        }


        public void Delete(TEntity entity)
        {
            if (entity != null)
            {
                _context.Attach(entity);
                _context.Remove(entity);
                _context.SaveChanges();
            }
        }

        public void Edit(TEntity entity)
        {
            if (entity != null)
            {
                _context.Update(entity);
                _context.SaveChanges();
            }
        }
        public void AddRange(List<TEntity> entities, bool isCommit = true)
        {
            try
            {
                _context.AddRange(entities);
                if (isCommit)
                    _context.SaveChanges();


            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void DeleteRange(List<TEntity> entities)
        {
            try
            {
                _context.RemoveRange(entities);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual Guid ParseGuid(string guidStr)
        {
            try
            {
                return Guid.Parse(guidStr);
            }
            catch { return Guid.Empty; }
        }

        public void SoftDeleteRange(List<TEntity> entities)
        {
            foreach (var item in entities)
            {
                item.IsDeleted = true;

            }
            _context.UpdateRange(entities);
            _context.SaveChanges();
        }


        #endregion
    }
}
