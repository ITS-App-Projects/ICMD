using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class SignalExtension : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string Extension { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string CBVariableType { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string PCS7VariableType { get; set; }

        [Column(TypeName = "character varying(5)")]
        [MaxLength(5)]
        public string Kind { get; set; }
    }
}
