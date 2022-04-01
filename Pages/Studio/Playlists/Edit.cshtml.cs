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

namespace MusicLibrary.Pages.Studio.Playlists
{
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

        public IList<Song> AllSongs { get; set; }

        public int currentPlaylistID { get; set; }

        public PlaylistSongs removePlaylistSongs { get; set; }


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
                _db.Database.ExecuteSqlRaw("Insert into [dbo].[PlaylistSongs] (PlaylistID, SongID) values({0}, {1}) ", currentPlaylistID, addSongID);
                return Redirect("./Edit?id=" + currentPlaylistID.ToString());
            }

            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _db.Attach(playlist).State = EntityState.Modified;

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

        private bool PlaylistExists(int id)
        {
            return _db.Playlist.Any(pl => pl.PlaylistID == id);
        }
    }
}
