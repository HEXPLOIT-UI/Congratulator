using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Congratulator.Models
{
    public class BirthdayModelDTO
    {
        [Required]
        public required string Id { get; set; }
        [Required]
        public required string FullName { get; set; }
        [Required]
        public required DateTimeOffset BirthdayDate { get; set; }
        [AllowNull]
        public string ProfileImageUri { get; set; }
    }
}
