using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Browse.Albums
{
    //[Authorize] Publicly accessible
    public class IndexModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public IndexModel(MusicLibraryContext context)
        {
            _db = context;
        }

        public IList<Album> Albums { get; set; }

        [BindProperty(SupportsGet = true)]

        public string searchString { get; set; }

        public async Task OnGet()
        {
            var albums = from al in _db.Album select al;

            if (!string.IsNullOrEmpty(searchString))
            {
                albums = albums.Where(al => al.AlbumName == searchString);
            }

            Albums = await albums.ToListAsync();
        }
    }
}