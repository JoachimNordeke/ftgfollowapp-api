namespace DocumentDb
{
    public class DeleteResult
    {
        public bool IsAcknowledged { get; set; }
        public long DeletedCount { get; set; }
    }
}
