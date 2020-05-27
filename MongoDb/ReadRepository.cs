using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DocumentDb;

namespace MongoDb
{
    internal class ReadRepository<TDocument> : IReadRepository<TDocument> where TDocument : Document
    {
        internal readonly IMongoCollection<TDocument> Collection;
        public ReadRepository(IMongoCollection<TDocument> collection)
        {
            Collection = collection;
        }

        public IEnumerable<TDocument> Find(Expression<Func<TDocument, bool>> where) =>
            Collection.Find(where).ToEnumerable();

        public async Task<IEnumerable<TDocument>> FindAsync(Expression<Func<TDocument, bool>> where) =>
            await Collection.Find(where).ToListAsync();

        public TDocument Get(string id) =>
            Collection.Find(x => x.Id == id).SingleOrDefault();

        public async Task<TDocument> GetAsync(string id) =>
            await Collection.Find(x => x.Id == id).SingleOrDefaultAsync();

        public IEnumerable<TDocument> GetAll() =>
            Collection.Find(x => true).ToEnumerable();

        public async Task<IEnumerable<TDocument>> GetAllAsync() =>
            await Collection.Find(x => true).ToListAsync();
    }
}
