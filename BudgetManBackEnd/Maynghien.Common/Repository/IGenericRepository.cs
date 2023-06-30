
using MayNghien.Common.Models;
using MayNghien.Common.Models.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Maynghien.Common.Repository
{
    public interface IGenericRepository<T, C> where T : BaseEntity where C : BaseContext
    {

        void Add(T entity);

        void Delete(T entity);

        void Edit(T entity);

        void AddRange(List<T> entities, bool isCommit = true);

        void DeleteRange(List<T> entities);
        void SoftDeleteRange(List<T> entities);

    }
}
