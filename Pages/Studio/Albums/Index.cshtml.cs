#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;
using Microsoft.AspNetCore.Authorization;

namespace MusicLibrary.Pages.Studio.Albums
{
    [Authorize]
	public class IndexModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public IndexModel(MusicLibraryContext context)
        {
            _db = context;
        }

        public string loggedInUserName { get; set; } = String.Empty;
        public IList<Album> Albums { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchString { get; set; }

        public async Task OnGet()
        {
            // Get currently logged in user name
            loggedInUserName = HttpContext.User.Identity.Name;

            // Get only albums that are created by current user
            var albums = from al in _db.Album.Where(al => al.ArtistName == loggedInUserName) select al;

            // If searchString is not empty, search for albums that searchString in its name (contains)
            if (!string.IsNullOrEmpty(searchString))
            {
                albums = albums.Where(al => al.AlbumName.Contains(searchString));
            }

            // Update albums display for front end
            Albums = await albums.ToListAsync();

        }
    }
}
