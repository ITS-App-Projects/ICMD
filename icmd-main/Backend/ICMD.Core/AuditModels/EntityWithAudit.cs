namespace ICMD.Core.AuditModels
{
    public abstract class EntityWithAudit<T> : CreationEntityWithAudit<T>
    {
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
