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

namespace MusicLibrary.Pages.Browse.Albums
{
    [Authorize]
    public class PlayModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        private readonly BlobService _blobService;

        public PlayModel(MusicLibraryContext context, BlobService _blobservice)
        {
            _db = context;
            _blobService = _blobservice;
        }
        public IList<Song> AlbumSongs { get; set; }
        public int currentSongIndex { get; set; }
        public Album Album { get; set; }
        public Song CurrentSong { get; set; }

        public string SongUri { get; set; }

        public async Task<IActionResult> OnGetAsync(int? albumID, int? songID, int? songIndex)
        {
            string loggedInUserName = HttpContext.User.Identity.Name;
            if (albumID == null)
            {
                return NotFound();
            }

            Album = await _db.Album.FirstOrDefaultAsync(al => al.AlbumID == albumID);

            if (Album == null)
            {
                return NotFound();
            }

            var albumSongs = from s in _db.Song.FromSqlRaw("SELECT S.* from dbo.Song S, dbo.Album AL, dbo.AlbumSongs PS"
                                                            + " WHERE AL.AlbumID = PS.AlbumID"
                                                            + " AND S.SongID = PS.SongID"
                                                            + " AND AL.AlbumID = {0}", albumID)
                             select s;

            AlbumSongs = await albumSongs.Distinct().ToListAsync();

            if (AlbumSongs.Count == 0)
            {
                CurrentSong = await _db.Song.FirstOrDefaultAsync(s => s.SongID == AlbumSongs[0].SongID);

                if (CurrentSong == null)
                {
                    return NotFound();
                }
                SongUri = await _blobService.GetFile(CurrentSong.FileName);
                _db.Database.ExecuteSqlRaw("Insert into [dbo].[View] (UserName, SongID, ViewDate) values({0}, {1}, {2}) ", loggedInUserName, AlbumSongs[0].SongID, DateTime.Now);
            }

            if (songID != null)
            {
                currentSongIndex = findSongIndex(AlbumSongs, songID);
                return Redirect(String.Format("./Play?songIndex={0}&albumID={1}", currentSongIndex, albumID));
            }

            if (songIndex != null)
            {
                currentSongIndex = (int)songIndex;
                await loadSong(AlbumSongs, songIndex);
            }

            return Page();
        }

        private async Task<IActionResult> loadSong(IList<Song> AlbumSongs, int? inputSongIndex)
        {
            string loggedInUserName = HttpContext.User.Identity.Name;
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
            _db.Database.ExecuteSqlRaw("Insert into [dbo].[View] (UserName, SongID, ViewDate) values({0}, {1}, {2}) ", loggedInUserName, AlbumSongs[localSongIndex].SongID, DateTime.Now);
            return Page();
        }
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