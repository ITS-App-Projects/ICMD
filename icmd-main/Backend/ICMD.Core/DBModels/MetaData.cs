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
    public class MetaData : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string Property { get; set; }

        public string? Value { get; set; }
    }
}
