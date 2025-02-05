namespace ICMD.Core.Common
{
    public class ImportFileResultDto<T>
    {
        public IReadOnlyList<T> Records { get; set; }
        public List<string>? Headers { get; set; }
        public string Message { get; set; }
        public bool IsSucceeded { get; set; } = false;
        public bool? IsWarning { get; set; }
    }
}
