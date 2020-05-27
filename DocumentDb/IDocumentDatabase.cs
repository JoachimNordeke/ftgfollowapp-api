namespace DocumentDb
{
    public interface IDocumentDatabase
    {
        IReadRepository<TDocument> GetReadRepository<TDocument>() where TDocument : Document;
        IWriteRepository<TDocument> GetWriteRepository<TDocument>() where TDocument : Document;
    }
}
