using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Shared.Extension
{
    public static class IdentityExtension
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {

            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return new Guid(principal?.FindFirstValue(IdentityClaimNames.UserId) ?? "");
        }
    }
}
