using System.ComponentModel.DataAnnotations;

namespace CloudCore.Contracts.Requests
{
    public record FolderCreateRequest
    {
        [Required(ErrorMessage = "Folder name is required")]
        [StringLength(250, MinimumLength = 1, ErrorMessage = "Folder name must be 1-250 characters")]
        [RegularExpression(@"^[^<>:""/\\|?*\x00-\x1F]+$",
            ErrorMessage = "Folder name contains invalid characters")]
        public required string Name { get; init; }

        [Range(1, int.MaxValue, ErrorMessage = "Parent folder ID must be positive")]
        public int? ParentId { get; init; }
    }
}