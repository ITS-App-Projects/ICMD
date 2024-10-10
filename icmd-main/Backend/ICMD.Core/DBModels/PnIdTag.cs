using ICMD.Core.AuditModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.DBModels
{
    public class PnIdTag : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string? Switches { get; set; }

        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string? PipelineTag { get; set; }

        public int? PnPId { get; set; }

        [Column(TypeName = "character varying(1)")]
        [MaxLength(1)]
        public string? Source { get; set; }

        public Guid TagId { get; set; }
        [ForeignKey("TagId")]
        public virtual Tag Tag { get; set; }

        public Guid DocumentReferenceId { get; set; }
        [ForeignKey("DocumentReferenceId")]
        public virtual ReferenceDocument DocumentReference { get; set; }

        public Guid? SkidId { get; set; }
        [ForeignKey("SkidId")]
        public virtual Skid? Skid { get; set; }

        public Guid? FailStateId { get; set; }
        [ForeignKey("FailStateId")]
        public virtual FailState? FailState { get; set; }
    }
}
