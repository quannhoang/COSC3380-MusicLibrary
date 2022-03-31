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

namespace MusicLibrary.Pages.Studio.Playlists
{
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
            loggedInUserName = HttpContext.User.Identity.Name;
            playlist.UserName = loggedInUserName;
            return Page();
        }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            
            if (!ModelState.IsValid) // Form inputs are invalid (example: name too short)
            {
                return Page();
            }
            _db.Playlist.Add(playlist);
            await _db.SaveChangesAsync();
            // FIXME: Add Else print some error 

            return RedirectToPage("./Index");

        }
    }
}
