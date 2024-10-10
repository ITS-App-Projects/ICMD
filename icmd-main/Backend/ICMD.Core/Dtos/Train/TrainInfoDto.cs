using ICMD.Core.Common;

namespace ICMD.Core.Dtos.Train
{
    public class TrainInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? Train { get; set; }
        public Guid ProjectId { get; set; }
    }
}
