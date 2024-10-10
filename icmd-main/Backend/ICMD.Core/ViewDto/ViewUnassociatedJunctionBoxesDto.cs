using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.ViewDto
{
    public class ViewUnassociatedJunctionBoxesDto
    {
        public Guid Id { get; set; }
        public Guid TagId { get; set; }
        public string? TagName { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public Guid? ReferenceDocumentId { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Revision { get; set; }
        public string? Version { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
