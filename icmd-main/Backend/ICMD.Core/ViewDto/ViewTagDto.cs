namespace ICMD.Core.ViewDto
{
    public class ViewTagDto
    {
        public Guid Id { get; set; }
        public string TagName { get; set; }
        public string? ProcessName { get; set; }
        public string? SubProcessName { get; set; }
        public string? StreamName { get; set; }
        public string? EquipmentCode { get; set; }
        public string? SequenceNumber { get; set; }
        public string? EquipmentIdentifier { get; set; }
        public Guid ProjectId { get; set; }
    }
}
