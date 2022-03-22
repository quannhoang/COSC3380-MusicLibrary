using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicLibrary.Models
{
    public class PlaylistSongs
    {
        //[Key]
        //public int PlayListSongsID { get; set; }

        public int? PlaylistID { get; set; }
        [ForeignKey("PlaylistID")]
        public Playlist? Playlist { get; set; }

        public int? SongID { get; set; }
        [ForeignKey("SongID")]
        public Song? Song { get; set; }
    }
}
