namespace ICMD.Core.ViewDto
{
    public class ViewAllAttributesDto
    {
        public Guid Id { get; set; }
        public Guid? AttributeDefinitionId { get; set; }
        public string? Name { get; set; }
        public string? ValueType { get; set; }
        public Guid? AttributeValueId { get; set; }
        public string? Value { get; set; }
    }
}
