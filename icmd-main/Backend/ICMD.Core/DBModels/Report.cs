using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class Report : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? Group { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "character varying(2000)")]
        [MaxLength(2000)]
        public string URL { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Description { get; set; }
        public int Order { get; set; }
    }
}
