using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Filter
{
    public class RequiredListAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is IList list)
            {
                return list.Count > 0;
            }

            return false;
        }
    }
}
