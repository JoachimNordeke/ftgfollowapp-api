using DocumentDb;

namespace API.Models.Users
{
    public class EmailValidation : Document
    {
        public string UserId { get; set; }
        public string ValidationCode { get; set; }
    }
}
