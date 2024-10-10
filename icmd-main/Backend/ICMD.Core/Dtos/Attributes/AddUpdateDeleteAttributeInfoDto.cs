using ICMD.Core.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Dtos.Attributes
{
    public class AddUpdateDeleteAttributeInfoDto
    {
        public List<AttributeDetailsChangeLogDto> NewAttributes { get; set; } = new List<AttributeDetailsChangeLogDto>();
        public List<AttributeDetailsChangeLogDto> ModifiedAttributes { get; set; } = new List<AttributeDetailsChangeLogDto>();
        public List<AttributeDetailsChangeLogDto> DeletedAttributes { get; set; } = new List<AttributeDetailsChangeLogDto>();
    }
}
