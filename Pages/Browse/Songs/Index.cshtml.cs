#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Browse.Songs
{
    public class IndexModel : PageModel
    {
        private readonly MusicLibraryContext _db;
        public IndexModel(MusicLibraryContext context)
        {
            _db = context;
        }

        public IList<Song> Songs { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchString { get; set; }
        public SelectList Genres { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchGenre { get; set; }
        public async Task OnGet()
        {
            IQueryable<string> genreQuery = from s in _db.Song
                                            orderby s.Genre
                                            select s.Genre;
            var songs = from s in _db.Song select s;
            if (!string.IsNullOrEmpty(searchString))
            {
                songs = songs.Where(song => song.Name.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(searchGenre))
            {
                songs = songs.Where(song => song.Genre == searchGenre);
            }
            Genres = new SelectList(await genreQuery.Distinct().ToListAsync());
            Songs = await songs.ToListAsync();

        }
    }
}
