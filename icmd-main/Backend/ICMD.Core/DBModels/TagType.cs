using ICMD.Core.AuditModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.DBModels
{
    public class TagType : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string Name { get; set; }

        [Column(TypeName = "character varying(80)")]
        public string? Description { get; set; }
    }
}
