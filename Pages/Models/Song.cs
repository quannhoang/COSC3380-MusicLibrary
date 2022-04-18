using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.Models
{
    public class Song
    {
        [Key]
        public int SongID { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Upload Date")]
        [Required]
        public DateTime UploadDate { get; set; } = DateTime.Today;

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Genre { get; set; } = string.Empty;

        public User? User { get; set; }

        [Required]
        [RegularExpression(@"[0-9a-zA-Z\s]*$", ErrorMessage = "Artist name must have alphanumeric letter only, and has 3-30 characters")]
        [StringLength(30, MinimumLength = 3)]
        [ForeignKey("User")]
        public string Artist { get; set; } = "DummyUserName" ;

        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Length { get; set; } = String.Empty;

        [Display(Name="Views")]
        [Required]
        public int ViewCount { get; set; } = 0;

        [Display(Name = "Likes")]
        [Required]
        public int LikeCount { get; set; } = 0;

        //[Required]
        [StringLength(50)]
        public string FileName { get; set; } = string.Empty;

        //public ICollection<Like>? Likes { get; set; }

        //public ICollection<PlaylistSongs>? PlaylistSongs { get; set; }

        //public ICollection<AlbumSongs>? AlbumSongs { get; set; }
    }
}
