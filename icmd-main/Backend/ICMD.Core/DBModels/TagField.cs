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
    public class TagField : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string? Name { get; set; }

        [Column(TypeName = "character varying(100)")]
        [MaxLength(100)]
        public string? Source { get; set; }

        [Column(TypeName = "character varying(20)")]
        [MaxLength(20)]
        public string? Separator { get; set; }

        public int Position { get; set; }

        public Guid ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

    }
}
