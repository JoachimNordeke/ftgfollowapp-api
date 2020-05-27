using System.Collections.Generic;
using DocumentDb;

namespace API.Models.Users
{
    public class User : Document
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Role { get; set; }
        public bool IsPasswordReset { get; set; }
        public bool IsActive { get; set; }

        public string MainStoreId { get; set; }
        public IEnumerable<string> StoreIds { get; set; }
    }
}
