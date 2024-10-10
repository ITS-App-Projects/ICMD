using System.ComponentModel.DataAnnotations;

namespace ICMD.API.Helpers
{
    public class CommonHelper
    {
        public Tuple<bool, List<string>> CheckImportFileRecordValidations(Object requestModel)
        {
            List<ValidationResult> validationResults = [];

            // Creating validation context
            var validationContext = new ValidationContext(requestModel);

            // Validating the model
            bool isValid = Validator.TryValidateObject(requestModel, validationContext, validationResults, true);

            List<string> Errors = validationResults.Where(x => !string.IsNullOrEmpty(x.ErrorMessage)).Select(x => x.ErrorMessage!).ToList() ?? [];
            return Tuple.Create(isValid, Errors);
        }
    }
}
