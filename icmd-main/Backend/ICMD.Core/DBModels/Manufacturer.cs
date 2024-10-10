using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class Manufacturer : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string Name { get; set; }

        [Column(TypeName = "character varying(255)")]

        public string? Description { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Comment { get; set; }
    }
}
