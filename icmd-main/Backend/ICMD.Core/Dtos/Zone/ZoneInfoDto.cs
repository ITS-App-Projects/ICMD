using ICMD.Core.Common;

namespace ICMD.Core.Dtos.Zone
{
    public class ZoneInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }

        public string? Zone { get; set; }

        public string? Description { get; set; }

        public string? Area { get; set; }

        public Guid ProjectId { get; set; }

    }
}
