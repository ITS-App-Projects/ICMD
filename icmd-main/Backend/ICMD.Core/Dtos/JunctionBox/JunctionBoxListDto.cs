using ICMD.Core.Common;

namespace ICMD.Core.Dtos.JunctionBox
{
    public class JunctionBoxListDto : ImportFileResponseDto
    {
        public Guid Id { get; set; }
        public string? Tag { get; set; }
        public string? Process { get; set; }
        public string? SubProcess { get; set; }
        public string? Stream { get; set; }
        public string? EquipmentCode { get; set; }
        public string? SequenceNumber { get; set; }
        public string? EquipmentIdentifier { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? ReferenceDocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Revision { get; set; }
        public string? Version { get; set; }
        public string? Sheet { get; set; }
        public bool IsVDPDocumentNumber { get; set; }
        public Guid? ProjectId { get; set; }
        public string? Number { get; set; }
        public bool IsActive { get; set; }

        public string? Area { get; set; }
    }
}
