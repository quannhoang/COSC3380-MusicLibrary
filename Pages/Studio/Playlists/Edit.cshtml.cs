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

        public int playlistID { get; set; }

        public PlaylistSongs removePlaylistSongs { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id, int? SongID)
        {
            if (id == null)
            {
                return NotFound();
            }
            playlistID = (int)id;
            playlist = await _db.Playlist.FirstOrDefaultAsync(pl => pl.PlaylistID == id);

            if (playlist == null)
            {
                return NotFound();
            }
            var playlistSongs = from s in _db.Song.FromSqlRaw("SELECT S.* from dbo.Song S, dbo.Playlist PL, dbo.PlaylistSongs PLS"
                                                            + " WHERE PL.PlaylistID = PLS.PlaylistID"
                                                            + " AND S.SongID = PLS.SongID"
                                                            + " AND PL.PlayListID = {0}", id)
                                select s;

            PlaylistSongs = await playlistSongs.Distinct().ToListAsync();

            if (SongID != null)
            {
                //removePlaylistSongs = _db.PlaylistSongs.FirstOrDefault(pls => pls.SongID == SongID && pls.PlaylistID == id);
                //_db.PlaylistSongs.Remove(removePlaylistSongs);
                //await _db.SaveChangesAsync();
                _db.Database.ExecuteSqlRaw("DELETE FROM PlaylistSongs WHERE SongID = {0} AND PlaylistID = {1}", SongID, id);
                return Redirect("./Edit?id=" + id.ToString());
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
