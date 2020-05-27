namespace API.Constants
{
    public static class Roles
    {
        public const string Admin = "admin";
        public const string RegionalManager = "regionalmanager";
        public const string StoreManager = "storemanager";
        public const string Seller = "seller";

        public static string JoinRoles(string[] roles)
        {
            return string.Join(',', roles);
        }
    }
}
