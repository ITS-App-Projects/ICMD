using ICMD.Core.AuditModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.DBModels
{
    public class DeviceModel : FullEntityWithAudit<Guid>
    {
        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string Model { get; set; }

        [Column(TypeName = "character varying(255)")]
        [MaxLength(255)]
        public string? Description { get; set; }
        public Guid ManufacturerId { get; set; }
        [ForeignKey("ManufacturerId")]
        public virtual Manufacturer Manufacturer { get; set; }
    }
}
