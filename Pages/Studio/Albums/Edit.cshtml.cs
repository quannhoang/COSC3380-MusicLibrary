#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;
using Microsoft.AspNetCore.Authorization;

namespace MusicLibrary.Pages.Studio.Albums
{
    [Authorize]
	public class EditModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public EditModel(MusicLibraryContext context)
        {
            _db = context;
        }

        [BindProperty]
        public Album album { get; set; }

        public IList<Song> AlbumSongs { get; set; }

        public IList<Song> InsertedSong { get; set; }

        public IList<Song> AllSongs { get; set; }

        public int currentAlbumID { get; set; }

        public AlbumSongs removeAlbumSongs { get; set; }

        public MusicLibrary.Models.User CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, int? removeSongID, int? addSongID)
        {
            if (id == null) // Null route, return not found page
            {
                return NotFound();
            }
            currentAlbumID = (int)id; //save id to currentAlbumID
            album = await _db.Album.FirstOrDefaultAsync(al => al.AlbumID == currentAlbumID); // Query the album from DB

            if (album == null) // If query return nothing, return not found page
            {
                return NotFound();
            }

            // If user try to edit album that is not owned by user, route to access denied
            if (album.ArtistName != HttpContext.User.Identity.Name)
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            // Select songs that are in the album (so user can remove from album if needed)
            var albumSongs = from s in _db.Song.FromSqlRaw("SELECT S.* from dbo.Song S, dbo.Album AL, dbo.AlbumSongs ALS"
                                                            + " WHERE AL.AlbumID = ALS.AlbumID"
                                                            + " AND S.SongID = ALS.SongID"
                                                            + " AND AL.AlbumID = {0}", currentAlbumID)
                             select s;

            AlbumSongs = await albumSongs.Distinct().ToListAsync();


            // Getting user username
            var loggedInUserName = HttpContext.User.Identity.Name;

            // Select all songs in the library that are not in the album and belong to the artist (so user can add to the album if needed)
            var allSongs = from s in _db.Song.FromSqlRaw("SELECT * FROM dbo.Song S"
                                                       + " WHERE S.SongID NOT IN"
                                                       + " (SELECT DISTINCT ALS.SongID FROM Album AL, AlbumSongs ALS "
                                                       + " WHERE AL.AlbumID = ALS.AlbumID "
                                                       + " AND ALS.AlbumID = {0})"
                                                       + " AND S.Artist  = {1}", currentAlbumID, loggedInUserName)
                           select s;

            AllSongs = allSongs.ToList();

            if (removeSongID != null) // If user want to remove song from album
            {
                _db.Database.ExecuteSqlRaw("DELETE FROM AlbumSongs WHERE SongID = {0} AND AlbumID = {1}", removeSongID, currentAlbumID);
                return Redirect("./Edit?id=" + currentAlbumID.ToString());
            }

            if (addSongID != null) // If user want to add song to album
            {
                // Check if song is already in album to make sure
                var insertedSongs = from s in _db.Song.FromSqlRaw("SELECT S.* from dbo.Song S, dbo.Album AL, dbo.AlbumSongs ALS"
                                                             + " WHERE AL.AlbumID = ALS.AlbumID"
                                                             + " AND S.SongID = ALS.SongID"
                                                             + " AND AL.AlbumID = {0}"
                                                             + " AND ALS.SongID = {1}", currentAlbumID, addSongID)
                                    select s;

                InsertedSong = await insertedSongs.Distinct().ToListAsync();

                // If song is not already in album, add it in album
                if (InsertedSong.Count() == 0)
                    _db.Database.ExecuteSqlRaw("Insert into [dbo].[AlbumSongs] (AlbumID, SongID) values({0}, {1}) ", currentAlbumID, addSongID);
                return Redirect("./Edit?id=" + currentAlbumID.ToString());
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() //This function is where album name can be changed (post method)
        {
            // If inputs are not correct, does nothing
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Change album 
            _db.Attach(album).State = EntityState.Modified;

            // Save to database
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumExists(album.AlbumID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        // Function to check if album exists in database using album id
        private bool AlbumExists(int id)
        {
            return _db.Album.Any(al => al.AlbumID == id);
        }
    }
}
