using System.ComponentModel.DataAnnotations;

using ICMD.Core.Constants;

namespace ICMD.Core.Dtos.CableAsset
{
    public class CreateOrEditCableAssetTypeDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid? ProjectId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(3, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? CableAssetType { get; set; }

        [DataType(DataType.Text)]
        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? CableAssetDescription { get; set; }

        [DataType(DataType.Text)]
        [StringLength(3, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? CableCategory { get; set; }

        [DataType(DataType.Text)]
        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? CableCategoryDescription { get; set; }

        [DataType(DataType.Text)]
        [StringLength(3, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? CableType { get; set; }

        [DataType(DataType.Text)]
        [StringLength(255, ErrorMessage = ResponseMessages.StringFieldLength, MinimumLength = 0)]
        public string? CableTypeDescription { get; set; }
    }
}
