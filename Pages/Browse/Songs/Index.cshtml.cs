#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Browse.Songs
{
    //[Authorize] Publicly accessible
    public class IndexModel : PageModel
    {
        private readonly MusicLibraryContext _db;
        public IndexModel(MusicLibraryContext context)
        {
            _db = context;
        }

        public string loggedInUserName { get; set; } = String.Empty;

        public IList<Song> Songs { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchString { get; set; }
        public SelectList Genres { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchGenre { get; set; }

        public IList<int> LikeList { get; set; } = new List<int>(new int[] { });
        public async Task<IActionResult> OnGetAsync(int? LikeSongID)
        {
            loggedInUserName = HttpContext.User.Identity.Name;
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


            foreach (var song in Songs)
            {
                var likeFromCurrentUser = _db.Like.FromSqlRaw("SELECT * FROM [dbo].[Like] WHERE SongID = {0} AND UserName = {1}", song.SongID, loggedInUserName);
                if (likeFromCurrentUser.Count() > 0)
                {
                    LikeList.Add(1);
                }
                else
                {
                    LikeList.Add(0);
                }
            }

            // NO LIKE FUNCTIONALITY ALLOWED FOR UNREGISTERD USERS
            //[Authorize]
            if (LikeSongID != null)
            {
                var LikeSong = from s in _db.Like.FromSqlRaw("SELECT * from [dbo].[Like] WHERE SongID = {0} AND UserName = {1}", LikeSongID, loggedInUserName) select s;
                IList<MusicLibrary.Models.Like> LikeSongList = await LikeSong.ToListAsync();
                if (LikeSongList.Count() == 0)
                {
                    _db.Database.ExecuteSqlRaw("Insert into [dbo].[Like] (UserName, SongID) values({0}, {1}) ", loggedInUserName, LikeSongID);
                    return RedirectToPage("./Index");
                }
                else
                {
                    _db.Database.ExecuteSqlRaw("DELETE FROM [dbo].[Like] WHERE UserName = {0} AND SongID = {1}", loggedInUserName, LikeSongID);
                    return RedirectToPage("./Index");
                }

            }
            return Page();
        }
    }
}
