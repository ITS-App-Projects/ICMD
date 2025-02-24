using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.UIChangeLog
{
    public class ChangeLogItemDto
    {
        public string? Type { get; set; }
        public string Tag { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? UserName { get; set; }
        public string? Status { get; set; }
        public List<PropertyChangeLogDto> Properties { get; set; } = new List<PropertyChangeLogDto>();
        public List<ReferenceDocumentChangeLogDto> ReferenceDocuments { get; set; } = new List<ReferenceDocumentChangeLogDto>();
        public List<PropertyChangeLogDto> Attributes { get; set; } = new List<PropertyChangeLogDto>();
        public List<PropertyChangeLogDto> Statuses { get; set; } = new List<PropertyChangeLogDto>();
        public List<BulkDeleteLogDto> BulkDeleteRecords { get; set; } = new List<BulkDeleteLogDto>();
        public List<ImportLogDto> ImportRecords { get; set; } = new List<ImportLogDto>();
    }


    public class ChangeLogResponceDto
    {
        public string Key { get; set; } = string.Empty;
        public List<ChangeLogItemDto> Items { get; set; }
    }

}
