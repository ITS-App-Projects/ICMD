using ICMD.Core.AuditModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class UIChangeLog : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(50)")]
        [MaxLength(50)]
        public string Tag { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string? PLCNumber { get; set; }

        public string Changes { get; set; }
        public string Type { get; set; }
    }
}
