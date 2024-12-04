namespace ICMD.Core.Dtos.Hierarchy
{
    public class HierarchyChildRequestDto
    {
        public Guid DeviceId { get; set; }
        public Guid? ProjectId { get; set; }
        public string? Option { get; set; }
        public string? HieararchyType { get; set; }
    }
}
