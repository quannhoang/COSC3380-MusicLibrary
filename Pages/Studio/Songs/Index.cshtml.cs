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

using Microsoft.AspNetCore.Authorization;

namespace MusicLibrary.Pages.Studio.Songs
{
    public class IndexModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public IndexModel(MusicLibraryContext context)
        {
            _db = context;
        }

        public string loggedInUserName { get; set; } = String.Empty;   
        public IList<Song> Songs { get;set; }
        [BindProperty(SupportsGet = true)]
        public string searchString { get; set; }
        public SelectList Genres { get; set; }
        //public List<string> Genres { get; set; }
        [BindProperty(SupportsGet = true)]
        public string searchGenre { get; set; }
        public async Task OnGet()
        {
            loggedInUserName = HttpContext.User.Identity.Name;
            Console.WriteLine(loggedInUserName);
            //IQueryable<string> genreQuery = from s in _db.Song.Where(s => s.Artist == loggedInUserName)
            IQueryable<string> genreQuery = from s in _db.Song.FromSqlRaw("SELECT * from dbo.Song WHERE Artist = {0}", loggedInUserName)
                                            orderby s.Genre
                                            select s.Genre;
            var songs = from s in _db.Song.FromSqlRaw("SELECT * from dbo.Song WHERE Artist = {0}", loggedInUserName) select s;
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
