using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class NatureOfSignal : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string NatureOfSignalName { get; set; }
    }
}
