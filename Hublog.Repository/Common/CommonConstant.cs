namespace Hublog.Repository.Common
{
    public class CommonConstant
    {
        public static class Policies
        {
            public const string EmployeePolicy = "EmployeePolicy"; 
            public const string AdminPolicy = "AdminPolicy"; 
            public const string UserOrAdminPolicy = "UserOrAdminPolicy";
        }

        public static class Role
        {
            public const string Employee = "EMPLOYEE";
            public const string Admin = "ADMIN";
            public const string SuperAdmin = "SUPER_ADMIN";
        }
    }
}
