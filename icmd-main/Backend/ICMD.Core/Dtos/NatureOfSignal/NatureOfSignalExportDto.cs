using ICMD.Core.Common;

namespace ICMD.Core.Dtos.NatureOfSignal
{
    public class NatureOfSignalExportDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
    }
}
