using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.Models
{
    public class AlbumSongs
    {
        public int? AlbumID { get; set; }
        [ForeignKey("AlbumID")]
        public Album? Album { get; set; }

        public int? SongID { get; set; }
        [ForeignKey("SongID")]
        public Song? Song { get; set; }
        
        

        
    }
}
