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

namespace MusicLibrary.Pages.Studio.Playlists
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
        public Playlist playlist { get; set; }

        public IList<Song> PlaylistSongs { get; set; }

        public IList<Song> InsertedSong { get; set; }

        public IList<Song> AllSongs { get; set; }

        public int currentPlaylistID { get; set; }

        public PlaylistSongs removePlaylistSongs { get; set; }

        public MusicLibrary.Models.User CurrentUser { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id, int? removeSongID, int? addSongID)
        {
            if (id == null) // Null route, return not found page
            {
                return NotFound();
            }
            currentPlaylistID = (int)id; //save id to currentPlaylistID
            playlist = await _db.Playlist.FirstOrDefaultAsync(pl => pl.PlaylistID == currentPlaylistID); // Query the playlist from DB

            if (playlist == null) // If query return nothing, return not found page
            {
                return NotFound();
            }

            // If user try to edit playlist that is not owned by user, route to access denied
            if (playlist.UserName != HttpContext.User.Identity.Name)
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            // Select songs that are in the playlist (so user can remove from playlist if needed)
            var playlistSongs = from s in _db.Song.FromSqlRaw("SELECT S.* from dbo.Song S, dbo.Playlist PL, dbo.PlaylistSongs PLS"
                                                            + " WHERE PL.PlaylistID = PLS.PlaylistID"
                                                            + " AND S.SongID = PLS.SongID"
                                                            + " AND PL.PlayListID = {0}", currentPlaylistID)
                                select s;

            PlaylistSongs = await playlistSongs.Distinct().ToListAsync();

            // Select all songs in the library that are not in the playlist (so user can add to the playlist if needed)
            var allSongs = from s in _db.Song.FromSqlRaw("SELECT * FROM dbo.Song S"
                                                       + " WHERE S.SongID NOT IN"
                                                       + " (SELECT DISTINCT PLS.SongID FROM Playlist PL, PlaylistSongs PLS "
                                                       + " WHERE PL.PlaylistID = PLS.PlaylistID "
                                                       + " AND PLS.PlaylistID = {0})", currentPlaylistID)
                           select s;

            AllSongs = allSongs.ToList();

            if (removeSongID != null) // If user want to remove song from playlist
            {
                _db.Database.ExecuteSqlRaw("DELETE FROM PlaylistSongs WHERE SongID = {0} AND PlaylistID = {1}", removeSongID, currentPlaylistID);
                return Redirect("./Edit?id=" + currentPlaylistID.ToString());
            }

            if (addSongID != null) // If user want to add song to playlist
            {
                // Check if song is already in playlist to make sure
                var insertedSongs = from s in _db.Song.FromSqlRaw("SELECT S.* from dbo.Song S, dbo.Playlist PL, dbo.PlaylistSongs PLS"
                                                             + " WHERE PL.PlaylistID = PLS.PlaylistID"
                                                             + " AND S.SongID = PLS.SongID"
                                                             + " AND PL.PlayListID = {0}"
                                                             + " AND PLS.SongID = {1}", currentPlaylistID, addSongID)
                                    select s;

                InsertedSong = await insertedSongs.Distinct().ToListAsync();
                
                // If song is not already in playlist, add it in playlist
                if (InsertedSong.Count()==0)
                    _db.Database.ExecuteSqlRaw("Insert into [dbo].[PlaylistSongs] (PlaylistID, SongID) values({0}, {1}) ", currentPlaylistID, addSongID);
                return Redirect("./Edit?id=" + currentPlaylistID.ToString());
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync() //This function is where playlist name can be changed (post method)
        {
            // If inputs are not correct, does nothing
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Change playlist 
            _db.Attach(playlist).State = EntityState.Modified;

            // Save to database
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlaylistExists(playlist.PlaylistID))
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

        
        // Function to check if playlist exists in database using playlist id
        private bool PlaylistExists(int id)
        {
            return _db.Playlist.Any(pl => pl.PlaylistID == id);
        }
    }
}
