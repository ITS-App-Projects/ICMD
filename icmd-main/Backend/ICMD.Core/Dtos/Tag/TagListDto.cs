namespace ICMD.Core.Dtos.Tag
{
    public class TagListDto
    {
        public Guid Id { get; set; }
        public string? Tag { get; set; }
        public string? Field1String { get; set; }
        public string? Field2String { get; set; }
        public string? Field3String { get; set; }
        public string? Field4String { get; set; }
        public string? Field5String { get; set; }
        public string? Field6String { get; set; }
        public Guid ProjectId { get; set; }
    }
}
