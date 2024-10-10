using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class ServiceTrain : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string Train { get; set; }
        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }
}
