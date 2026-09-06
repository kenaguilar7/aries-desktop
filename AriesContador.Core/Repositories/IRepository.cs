using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AriesContador.Core.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void Add(TEntity entity);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        Task Remove(TEntity entity);
    }
}
