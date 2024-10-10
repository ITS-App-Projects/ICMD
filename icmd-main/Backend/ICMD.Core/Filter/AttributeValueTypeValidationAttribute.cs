using ICMD.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICMD.Core.Filter
{
    public class AttributeValueTypeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value?.ToString()))
                return ValidationResult.Success;

            // get the value type property's value
            var property = validationContext.ObjectType.GetProperty("ValueType");
            var type = property.GetValue(validationContext.ObjectInstance, null).ToString();

            if (type == AttributeType.Text.ToString())
                return ValidationResult.Success;

            var result = true;

            // validate value as an integer if its type is Integer
            if (type == AttributeType.Integer.ToString())
            {
                int i;
                result = int.TryParse(value.ToString(), out i);

                if (result && i < 0)
                    result = false;

                return (result) ? ValidationResult.Success : new ValidationResult("Value must be a positive whole number");
            }
            // validate value as a decimal if its type is Decimal
            else if (type == AttributeType.Decimal.ToString())
            {
                float f;
                result = float.TryParse(value.ToString(), out f);

                return (result) ? ValidationResult.Success : new ValidationResult("Value must be a number");
            }

            return ValidationResult.Success;
        }
    }
}
