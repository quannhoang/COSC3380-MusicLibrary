#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;
using Microsoft.AspNetCore.Authorization;

namespace MusicLibrary.Pages.Studio.Playlists
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly MusicLibraryContext _db;


        public CreateModel(MusicLibraryContext context)
        {
            _db = context;
        }

        [BindProperty(SupportsGet = true)]
        public Playlist playlist { get; set; }

        public string loggedInUserName { get; set; } = string.Empty;

        public IActionResult OnGet()
        {
            // Get currently logged in username and assign it to new playlist
            loggedInUserName = HttpContext.User.Identity.Name;
            playlist.UserName = loggedInUserName;
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            // Form inputs are invalid (example: name too short) do nothing
            if (!ModelState.IsValid) 
            {
                return Page();
            }

            // If inputs are good, create playlist
            _db.Playlist.Add(playlist);
            await _db.SaveChangesAsync();
            // FIXME: Add Else print some error 

            return RedirectToPage("./Index");

        }
    }
}
