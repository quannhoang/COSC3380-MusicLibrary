using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MusicLibrary.Models
{
    public class Album
    {
        [Key]
        public int AlbumID { get; set; }

        [Required]
        [Display(Name = "Album Name")]
        [RegularExpression(@"[0-9a-zA-Z\s]*$", ErrorMessage = "Album name must have alphanumeric letter only, and has 3-30 characters")]
        [StringLength(30, MinimumLength = 3)]
        public string AlbumName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Create Date")]
        public DateTime CreateDate { get; set; } = DateTime.Today;

        [Display(Name = "Number of Songs")]
        public int SongCount { get; set; } = 0;

        public User? User { get; set; }

        [StringLength(30, MinimumLength = 3)]
        [ForeignKey("User")]
        public string ArtistName { get; set; } = string.Empty;
    }
}
