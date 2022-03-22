using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MusicLibrary.Models
{
    public class Like
    {
        /*public User? User { get; set; }

        [ForeignKey("User")]
        [StringLength(30, MinimumLength = 3)]
        public string? UserName { get; set; } = string.Empty;

        public int? SongId { get; set; }
        public Song? Song { get; set; }
        */
        [StringLength(30, MinimumLength = 3)]
        public string? UserName { get; set; } = string.Empty;

        [ForeignKey("UserName")]
        public User? User { get; set; }



        public int? SongID { get; set; }
        [ForeignKey("SongID")]
        public Song? Song { get; set; }
    }
}
