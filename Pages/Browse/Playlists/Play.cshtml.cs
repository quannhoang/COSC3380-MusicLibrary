﻿#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Browse.Playlists
{
    public class PlayModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        private readonly BlobService _blobService;

        public PlayModel(MusicLibraryContext context, BlobService blobService)
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
            if (playlistID == null) // If no playlisID provided, return not found page
            {
                return NotFound();
            }

            Playlist = await _db.Playlist.FirstOrDefaultAsync(pl => pl.PlaylistID == playlistID); // Query the playlist from DB

            if (Playlist == null) // If query returns nothing, return not found page
            {
                return NotFound();
            }

            // Select songs that are in the playlist (so user can remove from playlist if needed)
            var playlistSongs = from s in _db.Song.FromSqlRaw("SELECT S.* from dbo.Song S, dbo.Playlist PL, dbo.PlaylistSongs PLS"
                                                            + " WHERE PL.PlaylistID = PLS.PlaylistID"
                                                            + " AND S.SongID = PLS.SongID"
                                                            + " AND PL.PlayListID = {0}", playlistID)
                                select s;

            PlaylistSongs = await playlistSongs.Distinct().ToListAsync();
            // If Page is loaded the first time or no songID is provided, play the first song in the playlist
            if ((songID == null) && (songIndex == null))
            {
                CurrentSong = await _db.Song.FirstOrDefaultAsync(s => s.SongID == PlaylistSongs[0].SongID);

                if (CurrentSong == null)
                {
                    return NotFound();
                }
                SongUri = await _blobService.GetFile(CurrentSong.FileName);
            }

            // If user choose a song, play that song
            if (songID != null)
            {
                currentSongIndex = findSongIndex(PlaylistSongs, songID);
                return Redirect(String.Format("./Play?songIndex={0}&playlistID={1}", currentSongIndex, playlistID));
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