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
    public class DeleteModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public DeleteModel(MusicLibraryContext context)
        {
            _db = context;
        }

        [BindProperty]
        public Playlist playlist { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            playlist = await _db.Playlist.FirstOrDefaultAsync(p => p.PlaylistID == id);

            if (playlist == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            playlist = await _db.Playlist.FindAsync(id);

            if (playlist != null)
            {
                _db.Playlist.Remove(playlist);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
