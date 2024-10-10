using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ICMD.Core.DBModels
{
    public class Cable : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string Type { get; set; }

        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string OriginDescription { get; set; }

        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string DestDescription { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string Length { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(50)]
        public string CableRoute { get; set; }

        public Guid TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }

        public Guid OriginTagId { get; set; }
        [ForeignKey("OriginTagId")]
        public virtual Tag OriginTag { get; set; }

        public Guid DestTagId { get; set; }
        [ForeignKey("DestTagId")]
        public virtual Tag DestTag { get; set; }
    }
}
