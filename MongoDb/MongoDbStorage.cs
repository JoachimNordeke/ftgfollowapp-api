using MongoDB.Driver;
using DocumentDb;

namespace MongoDb
{
    public class MongoDbStorage : IMongoDbStorage
    {
        private readonly IMongoDatabase _database;

        public MongoDbStorage(string url, string databaseName)
        {
            var client = new MongoClient(url);
            _database = client.GetDatabase(databaseName);
        }

        public IReadRepository<TDocument> GetReadRepository<TDocument>() where TDocument : Document =>
            new ReadRepository<TDocument>(_database.GetCollection<TDocument>(typeof(TDocument).Name));

        public IWriteRepository<TDocument> GetWriteRepository<TDocument>() where TDocument : Document =>
            new WriteRepository<TDocument>(_database.GetCollection<TDocument>(typeof(TDocument).Name));
    }
}
