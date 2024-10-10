namespace ICMD.Core.ViewDto
{
    public class ViewAllDocumentsDto
    {
        public Guid DeviceId { get; set; }
        public string? Type { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Sheet { get; set; }
    }
}
