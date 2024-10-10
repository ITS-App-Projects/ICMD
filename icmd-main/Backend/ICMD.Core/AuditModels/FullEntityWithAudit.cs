namespace ICMD.Core.AuditModels
{
    public abstract class FullEntityWithAudit<T> : EntityWithAudit<T>
    {
        public bool IsDeleted { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
