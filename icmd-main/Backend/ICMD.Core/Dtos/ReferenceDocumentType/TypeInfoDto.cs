using ICMD.Core.Common;

namespace ICMD.Core.Dtos.ReferenceDocumentType
{
    public class TypeInfoDto: ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? Type { get; set; }
    }
}
