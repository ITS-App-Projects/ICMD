using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.Authorization
{
    [Table("ICMDUserRole")]
    public class ICMDUserRole : IdentityUserRole<Guid> { }

    [Table("ICMDRoleClaim")]
    public class ICMDRoleClaim : IdentityRoleClaim<Guid> { }

    [Table("ICMDUserClaim")]
    public class ICMDUserClaim : IdentityUserClaim<Guid> { }

    [Table("ICMDUserLogin")]
    public class ICMDUserLogin : IdentityUserLogin<Guid> { }

    [Table("ICMDUserToken")]
    public class ICMDUserToken : IdentityUserToken<Guid> { }


    [Table("ICMDRole")]
    public class ICMDRole : IdentityRole<Guid>
    {
        public ICMDRole()
        {
            DisplayName = "";
        }
        public string DisplayName { get; set; }
        public string Description { get; set; } = string.Empty;
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
