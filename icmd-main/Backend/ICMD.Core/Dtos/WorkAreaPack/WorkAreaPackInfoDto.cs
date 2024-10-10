using ICMD.Core.Common;

namespace ICMD.Core.Dtos.WorkAreaPack
{
    public class WorkAreaPackInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? Number { get; set; }
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
    }
}
