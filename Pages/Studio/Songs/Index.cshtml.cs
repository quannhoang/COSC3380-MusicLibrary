#nullable disable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Studio.Songs
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
        public Boolean liked { get; set; } = false;
        public IList<Song> Songs { get; set; }
        [BindProperty(SupportsGet = true)]
        public string searchString { get; set; }
        public SelectList Genres { get; set; }
        //public List<string> Genres { get; set; }
        [BindProperty(SupportsGet = true)]
        public string searchGenre { get; set; }
        public async Task OnGet(int? LikeSongID)
        {
            loggedInUserName = HttpContext.User.Identity.Name;
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

            if (LikeSongID != null)
            {
                var LikeSong = from s in _db.Like.FromSqlRaw("SELECT * from [dbo].[Like] WHERE SongID = {0} AND UserName = {1}", LikeSongID, loggedInUserName) select s;
                IList<MusicLibrary.Models.Like> LikeSongList = await LikeSong.ToListAsync();
                if (LikeSongList.Count() == 0)
                {
                    _db.Database.ExecuteSqlRaw("Insert into [dbo].[Like] (UserName, SongID) values({0}, {1}) ", loggedInUserName, LikeSongID);
                    liked = true;
                }
                else
                {
                    _db.Database.ExecuteSqlRaw("DELETE FROM [dbo].[Like] WHERE UserName = {0} AND SongID = {1}", loggedInUserName, LikeSongID);
                    liked = false;
                }

            }

        }

    }
}
