using ICMD.Core.AuditModels;

namespace ICMD.Core.DBModels
{
    public class CableAssetType : FullEntityWithAudit<Guid>
    {
        public required Guid ProjectId { get; set; }

        public required string Type { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string CategoryDescription { get; set; } = string.Empty;

        public string CableType { get; set; } = string.Empty;

        public string CableTypeDescription { get; set; } = string.Empty;
    }
}
