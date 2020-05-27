using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DocumentDb
{
    public interface IReadRepository<TDocument> where TDocument : Document
    {
        IEnumerable<TDocument> GetAll();

        Task<IEnumerable<TDocument>> GetAllAsync();

        IEnumerable<TDocument> Find(Expression<Func<TDocument, bool>> where);

        Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> where);

        TDocument Get(string id);

        Task<TDocument> GetAsync(string id);
    }
}
