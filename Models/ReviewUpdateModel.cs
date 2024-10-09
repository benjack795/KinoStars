using System.ComponentModel.DataAnnotations;

namespace KinoStars.Models
{
    public class ReviewUpdateModel
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

        [Required]
        public String OldPath { get; set; }   

        public static string GetImageFormat(byte[] bytes)
            {
                var png    = new byte[] { 137, 80, 78, 71 };    // PNG
                var jpeg   = new byte[] { 255, 216, 255, 224 }; // jpeg
                var jpeg2  = new byte[] { 255, 216, 255, 225 }; // jpeg canon

                if (png.SequenceEqual(bytes.Take(png.Length)))
                    return "png";

                if (jpeg.SequenceEqual(bytes.Take(jpeg.Length)))
                    return "jpeg";

                if (jpeg2.SequenceEqual(bytes.Take(jpeg2.Length)))
                    return "jpeg";

                return "invalid";
            } 
        
    }
}
