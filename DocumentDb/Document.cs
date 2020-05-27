using System;

namespace DocumentDb
{
    public class Document
    {
        public string Id { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
