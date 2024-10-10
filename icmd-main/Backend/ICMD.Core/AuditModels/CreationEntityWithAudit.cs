using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ICMD.Core.AuditModels
{
    public abstract class CreationEntityWithAudit<T>
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
