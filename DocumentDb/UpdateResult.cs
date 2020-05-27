namespace DocumentDb
{
    public class UpdateResult
    {
        public bool IsAcknowledged { get; set; }
        public long MatchedCount { get; set; }
        public long ModifiedCount { get; set; }
    }
}
