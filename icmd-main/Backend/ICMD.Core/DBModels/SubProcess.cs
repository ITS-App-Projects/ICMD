﻿using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class SubProcess : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string SubProcessName { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string Description { get; set; }
        public Guid ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
    }
}