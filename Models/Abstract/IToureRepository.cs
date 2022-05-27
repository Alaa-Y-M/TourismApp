using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.Abstract
{
    public interface IToureRepository<TEntity>
    {
        List<TEntity> List();
        List<TEntity> Search(string entity);
        TEntity Find(Guid id);
        void Add(TEntity entity);
        void Delete(Guid id);
        void Update(Guid id, TEntity entity);

    }
}
