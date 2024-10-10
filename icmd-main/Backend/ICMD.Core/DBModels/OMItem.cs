using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class OMItem : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string ItemId { get; set; }

        [Column(TypeName = "character varying(500)")]
        [MaxLength(500)]
        public string ItemDescription { get; set; }

        [Column(TypeName = "character varying(25)")]
        [MaxLength(25)]
        public string ParentItemId { get; set; }

        [Column(TypeName = "character varying(10)")]
        [MaxLength(10)]
        public string AssetTypeId { get; set; }

        public Guid ProjectId { get; set; }
    }
}
