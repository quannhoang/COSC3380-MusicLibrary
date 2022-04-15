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

namespace MusicLibrary.Pages.Browse.Playlists
{
    [Authorize]
    public class PlayPLModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        private readonly BlobService _blobService;

        public PlayPLModel(MusicLibraryContext context, BlobService blobService)
        {
            _db = context;
            _blobService = blobService;
        }

        public IList<Song> PlaylistSongs { get; set; }

        public int currentSongIndex { get; set; }

        public Playlist Playlist { get; set; }

        public Song CurrentSong { get; set; }

        public string SongUri { get; set; }

        public async Task<IActionResult> OnGetAsync(int? playlistID, int? songID, int? songIndex)
        {
            string loggedInUserName = HttpContext.User.Identity.Name;
            if (playlistID == null) // If no playlisID provided, return not found page
            {
                return NotFound();
            }

            Playlist = await _db.Playlist.FirstOrDefaultAsync(pl => pl.PlaylistID == playlistID); // Query the playlist from DB

            if (Playlist == null) // If query returns nothing, return not found page
            {
                return NotFound();
            }

            // Select songs that are in the playlist
            var playlistSongs = from s in _db.Song.FromSqlRaw("SELECT S.* from dbo.Song S, dbo.Playlist PL, dbo.PlaylistSongs PLS"
                                                            + " WHERE PL.PlaylistID = PLS.PlaylistID"
                                                            + " AND S.SongID = PLS.SongID"
                                                            + " AND PL.PlayListID = {0}", playlistID)
                                select s;

            PlaylistSongs = await playlistSongs.Distinct().ToListAsync();
            if (PlaylistSongs.Count == 0) return Page();
            // If Page is loaded the first time or no songID is provided, play the first song in the playlist
            if ((songID == null) && (songIndex == null))
            {
                CurrentSong = await _db.Song.FirstOrDefaultAsync(s => s.SongID == PlaylistSongs[0].SongID);

                if (CurrentSong == null)
                {
                    return NotFound();
                }
                SongUri = await _blobService.GetFile(CurrentSong.FileName);
                _db.Database.ExecuteSqlRaw("Insert into [dbo].[View] (UserName, SongID, ViewDate) values({0}, {1}, {2}) ", loggedInUserName, PlaylistSongs[0].SongID, DateTime.Now);
            }

            // If user choose a song, play that song
            if (songID != null)
            {
                currentSongIndex = findSongIndex(PlaylistSongs, songID);
                return Redirect(String.Format("./PlayPL?songIndex={0}&playlistID={1}", currentSongIndex, playlistID));
            }

            // If user choose Next, Or Previous, play based on songIndex
            if (songIndex != null)
            {
                currentSongIndex = (int)songIndex;
                await loadSong(PlaylistSongs, songIndex);
            }

            return Page();
        }

        // Function to load and play song based on songIndex
        private async Task<IActionResult> loadSong(IList<Song> PLaylistSongs, int? inputSongIndex)
        {
            string loggedInUserName = HttpContext.User.Identity.Name;
            int localSongIndex = (int)inputSongIndex;
            if (localSongIndex < 0) localSongIndex = PLaylistSongs.Count() - 1;
            if (localSongIndex >= PLaylistSongs.Count) localSongIndex = 0;
            currentSongIndex = localSongIndex;
            CurrentSong = await _db.Song.FirstOrDefaultAsync(s => s.SongID == PLaylistSongs[localSongIndex].SongID);

            if (CurrentSong == null)
            {
                return NotFound();
            }
            SongUri = await _blobService.GetFile(CurrentSong.FileName);
            _db.Database.ExecuteSqlRaw("Insert into [dbo].[View] (UserName, SongID, ViewDate) values({0}, {1}, {2}) ", loggedInUserName, PLaylistSongs[localSongIndex].SongID, DateTime.Now);
            return Page();
        }

        // Function to retrieve song's index in playlist using SongID
        private int findSongIndex(IList<Song> PlayListSongs, int? inputSongID)
        {
            int index = 0;
            foreach (var song in PlayListSongs)
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
