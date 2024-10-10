namespace ICMD.API.Helpers
{
    public class FileUploadModel
    {
        public IFormFile? File { get; set; }
        public Guid ProjectId { get; set; }
    }
}
