using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Congratulator.Models
{
    public class BirthdayModel
    {
       
        public ObjectId Id { get; set; }
        [Required(ErrorMessage = "cannot be null")]
        [Display(Name = "Имя")]
        [MinLength(3)]
        public required string FullName { get; set; }
        [Required(ErrorMessage = "cannot be null")]
        [Display(Name = "Дата")]
        public required DateTimeOffset BirthdayDate { get; set; }
    }
}
