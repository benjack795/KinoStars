using System.ComponentModel.DataAnnotations;

namespace KinoStars.Models
{
    public class ReviewViewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        public string ReviewText { get; set; }

        [Required]
        [Range(1,5, ErrorMessage = "Score must be between 1 and 5.")]
        public int Score { get; set; }

        [Required]
        public IFormFile ReviewPhoto { get; set; }
        
    }
}
