#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;
using Microsoft.AspNetCore.Authorization;

namespace MusicLibrary.Pages.Studio.Playlists
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
        public IList<Playlist> Playlists { get;set; }

        [BindProperty(SupportsGet = true)]
        public string searchString { get; set; }

        public async Task OnGet()
        {
            loggedInUserName = HttpContext.User.Identity.Name;
            var playlists = from pl in _db.Playlist.Where(pl => pl.UserName == loggedInUserName) select pl;
            if (!string.IsNullOrEmpty(searchString))
            {
                playlists = playlists.Where(pl => pl.PlaylistName.Contains(searchString));
            }
            Playlists = await playlists.ToListAsync();

        }

    }
}
