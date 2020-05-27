using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DocumentDb
{
    public interface IWriteRepository<TDocument> : IReadRepository<TDocument> where TDocument : Document
    {
        void Create(TDocument document);

        Task CreateAsync(TDocument document);

        void Update(TDocument document);

        Task<UpdateResult> UpdateAsync(TDocument document);

        void UpdatePartial(string id, object updateObject);

        Task<UpdateResult> UpdatePartialAsync(string id, object updateObject);

        bool AddToSet<TItem>(string documentId, string setName, TItem item);

        Task<bool> AddToSetAsync<TItem>(string documentId, string setName, TItem item);

        void Delete(string id);

        Task<DeleteResult> DeleteAsync(string id);

        Task<DeleteResult> DeleteManyAsync(Expression<Func<TDocument, bool>> predicate);
    }
}
