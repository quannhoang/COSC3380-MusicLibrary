using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MusicLibrary.Models
{
    public class View
    {
        [Key]
        public int ViewID { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string? UserName { get; set; } = string.Empty;
        [ForeignKey("UserName")]
        public User? User { get; set; }


        public int? SongID { get; set; }
        [ForeignKey("SongID")]
        public Song? Song { get; set; }

        public DateTime ViewDate { get; set; } = DateTime.Now;

        
        
    }
}
