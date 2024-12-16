using ICMD.Core.Common;

namespace ICMD.Core.Dtos.CableAsset
{
    public class CableAssetTypeInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string? CableAssetType { get; set; }
        public string? CableAssetDescription { get; set; }
        public string? CableCategory { get; set; }
        public string? CableCategoryDescription { get; set; }
        public string? CableType { get; set; }
        public string? CableTypeDescription { get; set; }
    }
}
