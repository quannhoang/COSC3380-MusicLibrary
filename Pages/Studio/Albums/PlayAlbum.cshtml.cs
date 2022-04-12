#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;
using Microsoft.AspNetCore.Authorization;
namespace MusicLibrary.Pages.Studio.Albums
{
    [Authorize]
	public class PlayAlbumModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        private readonly BlobService _blobService;

        public PlayAlbumModel(MusicLibraryContext context, BlobService blobService)
        {
            _db = context;
            _blobService = blobService;
        }

        public IList<Song> AlbumSongs { get; set; }

        public int currentSongIndex { get; set; }

        public Album Album { get; set; }

        public Song CurrentSong { get; set; }

        public string SongUri { get; set; }

        public async Task<IActionResult> OnGetAsync(int? albumID, int? songID, int? songIndex)
        {
            if (albumID == null) // If no AlbumID provided, return not found page
            {
                return NotFound();
            }

            Album = await _db.Album.FirstOrDefaultAsync(al => al.AlbumID == albumID); // Query the album from DB

            if (Album == null) // If query returns nothing, return not found page
            {
                return NotFound();
            }

            // Select songs that are in the album 
            var albumSongs = from s in _db.Song.FromSqlRaw("SELECT S.* from dbo.Song S, dbo.Album AL, dbo.AlbumSongs ALS"
                                                            + " WHERE AL.AlbumID = ALS.AlbumID"
                                                            + " AND S.SongID = ALS.SongID"
                                                            + " AND AL.AlbumID = {0}", albumID)
                                select s;

            AlbumSongs = await albumSongs.Distinct().ToListAsync();
            if (AlbumSongs.Count == 0) // If Album is empty
            {
                return Page();
            }
            // If Page is loaded the first time or no songID is provided, play the first song in the album
            if ((songID == null) && (songIndex == null))
            {
                CurrentSong = await _db.Song.FirstOrDefaultAsync(s => s.SongID == AlbumSongs[0].SongID);

                if (CurrentSong == null)
                {
                    return NotFound();
                }
                SongUri = await _blobService.GetFile(CurrentSong.FileName);
            }

            // If user choose a song, play that song
            if (songID != null)
            {
                currentSongIndex = findSongIndex(AlbumSongs, songID);
                return Redirect(String.Format("./Play?songIndex={0}&playlistID={1}", currentSongIndex, albumID));
            }

            // If user choose Next, Or Previous, play based on songIndex
            if (songIndex != null)
            {
                currentSongIndex = (int)songIndex;
                await loadSong(AlbumSongs, songIndex);
            }

            return Page();
        }

        // Function to load and play song based on songIndex
        private async Task<IActionResult> loadSong(IList<Song> AlbumSongs, int? inputSongIndex)
        {
            int localSongIndex = (int)inputSongIndex;
            if (localSongIndex < 0) localSongIndex = AlbumSongs.Count() - 1;
            if (localSongIndex >= AlbumSongs.Count) localSongIndex = 0;
            currentSongIndex = localSongIndex;
            CurrentSong = await _db.Song.FirstOrDefaultAsync(s => s.SongID == AlbumSongs[localSongIndex].SongID);

            if (CurrentSong == null)
            {
                return NotFound();
            }
            SongUri = await _blobService.GetFile(CurrentSong.FileName);
            return Page();
        }

        // Function to retrieve song's index in album using SongID
        private int findSongIndex(IList<Song> AlbumSongs, int? inputSongID)
        {
            int index = 0;
            foreach (var song in AlbumSongs)
            {
                if (song.SongID == inputSongID)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
    }
}
