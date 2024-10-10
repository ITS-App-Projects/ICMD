using ICMD.Core.DBModels;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.Authorization
{
    [Table("ICMDUsers")]
    public class ICMDUser : IdentityUser<Guid>
    {
        [Column(TypeName = "character varying(50)")]
        public string FirstName { get; set; } = string.Empty;

        [Column(TypeName = "character varying(50)")]
        public string LastName { get; set; } = string.Empty;

        [NotMapped]
        public string FullName => $@"{FirstName} {LastName}";
        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }
    }
}
