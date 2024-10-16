using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class OMServiceDescriptionImport : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string Tag { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string ServiceDescription { get; set; }

        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string? Area { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? Stream { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Bank { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? Service { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? Variable { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Train { get; set; }
        public Guid? ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }
}
