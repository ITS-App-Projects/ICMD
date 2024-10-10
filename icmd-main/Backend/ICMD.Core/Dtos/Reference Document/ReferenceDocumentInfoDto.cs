using ICMD.Core.Common;

namespace ICMD.Core.Dtos.Reference_Document
{
    public class ReferenceDocumentInfoDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? DocumentNumber { get; set; }
        public Guid? ReferenceDocumentTypeId { get; set; }
        public string? ReferenceDocumentType { get; set; }
        public string URL { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Revision { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Sheet { get; set; } = string.Empty;
        public Guid ProjectId { get; set; }
    }
}
