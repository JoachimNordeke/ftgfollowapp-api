using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API.Models.Users
{
    public class NewUpdateUser
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        public string MainStoreId { get; set; }
        public string Role { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Phone { get; set; }

        public IEnumerable<string> StoreIds { get; set; }
    }
}
