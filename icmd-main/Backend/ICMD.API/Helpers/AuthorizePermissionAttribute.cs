using ICMD.Core.Constants;
using Microsoft.AspNetCore.Mvc;

namespace ICMD.API.Helpers
{
    public class AuthorizePermissionAttribute : TypeFilterAttribute
    {
        public AuthorizePermissionAttribute(params Operations[] operation)
            : base(typeof(PermissionFilter))
        {
            Arguments = new[] { new PermissionRequirement(operation) };
        }
    }
}
