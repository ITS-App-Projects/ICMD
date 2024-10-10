using ICMD.Core.Common;

namespace ICMD.Core.Dtos.System
{
    public class SystemInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? Number { get; set; }
        public string? Description { get; set; }
        public string? WorkAreaPack { get; set; }
        public Guid WorkAreaPackId { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
