namespace ICMD.Core.Dtos.ImportValidation
{
    public class ValidationDataDto
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public List<ChangesDto> Changes { get; set; } = [];
    }

    public class ChangesDto
    {
        public string ItemColumnName { get; set; } = string.Empty;
        public string PreviousValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
    }
}
