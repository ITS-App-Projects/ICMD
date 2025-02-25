using ICMD.Core.Dtos.ImportValidation;

namespace ICMD.Core.Dtos.UIChangeLog
{
    public class ImportLogDto
    {
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Message { get; set; }
        public string? Operation { get; set; }
        public List<ChangesDto> Items { get; set; } = [];
    }
}
