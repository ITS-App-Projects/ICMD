using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Filter
{
    public class UrlValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value?.ToString()))
            {
                // Null values are considered valid; you can change this behavior if needed.
                return true;
            }

            if (value is string url)
            {
                Uri result;
                return Uri.TryCreate(url, UriKind.Absolute, out result);
            }

            // If the value is not a string, it's considered invalid.
            return false;
        }
    }
}
