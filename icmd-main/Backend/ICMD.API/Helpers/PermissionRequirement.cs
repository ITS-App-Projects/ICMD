using ICMD.Core.Constants;
using Microsoft.AspNetCore.Authorization;

namespace ICMD.API.Helpers
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(Operations[] operation)
        {
            Operation = operation;
        }
        public Operations[] Operation { get; set; }
    }
}
