using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Report
{
    public class ArtistProfileModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public ArtistProfileModel(MusicLibraryContext context)
        {
            _db = context;
        }

        public SelectList ArtistList { get; set; }

        [BindProperty]
        public string inputArtistName { get; set; } = string.Empty;
        public IList<Song> SongList { get; set; }
        public IList<Album> AlbumsList { get; set; }
        public IList<View> ViewList { get; set; }
        public IList<Like> LikeList { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            // When loading the page, get all artist names for selection box in front end
            IQueryable<string> allArtist = from u in _db.User.FromSqlRaw("SELECT * FROM [dbo].[User] WHERE isArtist=1")
                            orderby u.UserName
                            select u.UserName;
            ArtistList =  new SelectList(await allArtist.ToListAsync());
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // If no artist name provided, return to Page
            if (inputArtistName == null) return await OnGetAsync();

            // Get all artist names for selection box in front end
            IQueryable<string> allArtist = from u in _db.User.FromSqlRaw("SELECT * FROM [dbo].[User] WHERE isArtist=1")
                                           orderby u.UserName
                                           select u.UserName;
            ArtistList = new SelectList(await allArtist.ToListAsync());

            //

            return Page();
        }
    }
}
