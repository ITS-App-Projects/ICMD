using ICMD.Core.Common;

namespace ICMD.Core.Dtos.Process
{
    public class InstrumentInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string DeviceType { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
    }
}
