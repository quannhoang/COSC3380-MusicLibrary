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

namespace MusicLibrary.Pages.Studio.Albums
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
        public Album album { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // If album id is not provided return not found
            if (id == null)
            {
                return NotFound();
            }

            // Querry album from database
            album = await _db.Album.FirstOrDefaultAsync(a => a.AlbumID == id);

            // If user try to delete album that is not owned by user, route to access denied
            if (album.ArtistName != HttpContext.User.Identity.Name)
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            // If querry can not find the album, return not found
            if (album == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            // If album id is not provided, return not found
            if (id == null)
            {
                return NotFound();
            }

            // Querry album from database
            album = await _db.Album.FindAsync(id);

            // If querry can find album
            if (album != null)
            {
                // Remove that album from database
                _db.Album.Remove(album);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
