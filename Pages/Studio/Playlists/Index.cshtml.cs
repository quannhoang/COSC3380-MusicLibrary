#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Studio.Playlists
{
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
