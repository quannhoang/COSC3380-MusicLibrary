#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Browse.Playlists
{
    public class IndexModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public IndexModel(MusicLibraryContext context)
        {
            _db = context;
        }
        public IList<Playlist> Playlists { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchString { get; set; }

        public async Task OnGet()
        {
            var playlists = from pl in _db.Playlist select pl;
            if (!string.IsNullOrEmpty(searchString))
            {
                playlists = playlists.Where(pl => pl.PlaylistName.Contains(searchString));
            }
            Playlists = await playlists.ToListAsync();

        }
    }
}
