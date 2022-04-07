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

namespace MusicLibrary.Pages.Studio.Playlists
{
    [Authorize]
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
            // If playlist id is not provided return not found
            if (id == null)
            {
                return NotFound();
            }

            // Querry playlist from database
            playlist = await _db.Playlist.FirstOrDefaultAsync(p => p.PlaylistID == id);

            // If user try to delete playlist that is not owned by user, route to access denied
            if (playlist.UserName != HttpContext.User.Identity.Name)
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            // If querry can not find the playlist, return not found
            if (playlist == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            // If playlist id is not provided, return not found
            if (id == null)
            {
                return NotFound();
            }

            // Querry playlist from database
            playlist = await _db.Playlist.FindAsync(id);

            // If querry can find playlist
            if (playlist != null)
            {   
                // Remove that playlist from database
                _db.Playlist.Remove(playlist);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
