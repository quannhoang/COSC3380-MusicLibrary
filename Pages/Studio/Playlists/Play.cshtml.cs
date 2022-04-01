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

namespace MusicLibrary.Pages.Studio.Playlists
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

        public Song CurrentSong { get; set; }

        public IList<Song> PlaylistSongs { get; set; }

        public int currentPlaylistIndex { get; set; }

        public Playlist Playlist { get; set; }

        [BindProperty]
        public string SongUri { get; set; }

        public async Task<IActionResult> OnGetAsync(int? playlistID, int? songID, int? prev, int? next)
        {
            if (playlistID == null)
            {
                return NotFound();
            }

            Playlist = await _db.Playlist.FirstOrDefaultAsync(pl => pl.PlaylistID == playlistID); // Query the playlist from DB

            if (Playlist == null) // If query return nothing, return not found page
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
            currentPlaylistIndex = findSongIndex(PlaylistSongs, CurrentSong);
            // If Page is loaded the first time or no songID is provided, play the first song in the playlist
            if ((songID == null)&&(next==null)&&(prev==null))
            {
                Console.WriteLine(currentPlaylistIndex);
                CurrentSong = await _db.Song.FirstOrDefaultAsync(s => s.SongID == PlaylistSongs[0].SongID);

                if (CurrentSong == null)
                {
                    return NotFound();
                }
                SongUri = await _blobService.GetFile(CurrentSong.FileName);
                currentPlaylistIndex = findSongIndex(PlaylistSongs, CurrentSong);
                Console.WriteLine(currentPlaylistIndex);
            }

            // If user choose a song, play that song
            if (songID != null)
            {
                CurrentSong = await _db.Song.FirstOrDefaultAsync(s => s.SongID == songID);

                if (CurrentSong == null)
                {
                    return NotFound();
                }
                SongUri = await _blobService.GetFile(CurrentSong.FileName);
                currentPlaylistIndex = findSongIndex(PlaylistSongs, CurrentSong);
                Console.WriteLine(currentPlaylistIndex);
            }

            if (next != null)
            {
                currentPlaylistIndex++;
                Console.WriteLine(currentPlaylistIndex);
                if (currentPlaylistIndex >= PlaylistSongs.Count())
                    currentPlaylistIndex = 0;
                CurrentSong = await _db.Song.FirstOrDefaultAsync(s => s.SongID == PlaylistSongs[currentPlaylistIndex].SongID);

                if (CurrentSong == null)
                {
                    return NotFound();
                }
                SongUri = await _blobService.GetFile(CurrentSong.FileName);
            }

            if (prev != null)
            {
                currentPlaylistIndex--;
                Console.WriteLine(currentPlaylistIndex);
                if (currentPlaylistIndex < 0)
                    currentPlaylistIndex = PlaylistSongs.Count()-1;
                CurrentSong = await _db.Song.FirstOrDefaultAsync(s => s.SongID == PlaylistSongs[currentPlaylistIndex].SongID);

                if (CurrentSong == null)
                {
                    return NotFound();
                }
                SongUri = await _blobService.GetFile(CurrentSong.FileName);
            }

            return Page();
        }

        /*private int findSongIndex(IList<Song> PlayListSongs, int? inputSongID)
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
        }*/

        private int findSongIndex(IList<Song> PlayListSongs, Song? inputSong)
        {
            int index = 0;
            if (inputSong == null) return 0;
            foreach (var song in PlayListSongs)
            {
                if (song.SongID == inputSong.SongID)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }
    }
}
