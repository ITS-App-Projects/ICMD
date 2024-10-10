namespace ICMD.Core.Constants
{
    public static class RoleConstants
    {
        public const string Administrator = "Administrator";
        public const string SuperUser = "SuperUser";
        public const string User = "User";
    }

    public static class RoleOrder
    {
        public static readonly Dictionary<string, int> Roles = new()
        {
            { RoleConstants.Administrator, 1 },
            { RoleConstants.SuperUser, 2 },
            { RoleConstants.User, 3 }
        };
    }
}
