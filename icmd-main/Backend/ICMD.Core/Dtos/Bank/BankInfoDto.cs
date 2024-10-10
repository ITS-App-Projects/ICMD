using ICMD.Core.Common;

namespace ICMD.Core.Dtos.Bank
{
    public class BankInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? Bank { get; set; }
        public Guid ProjectId { get; set; }
    }
}
