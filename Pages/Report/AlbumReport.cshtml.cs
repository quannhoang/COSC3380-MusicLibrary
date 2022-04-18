using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace MusicLibrary.Pages.Report
{
    public class AlbumReportModel : PageModel
    {

        private readonly MusicLibraryContext _db; // Database object

        public AlbumReportModel(MusicLibraryContext context) //Database object init
        {
            _db = context;
        }

        public IList<Album> AllAlbums { get; set; }
        public int AllAlbumsSize { get; set; } = 0;
        public IList<Song> AllSongs { get; set; }

        public string fromDate { get; set; }

        public string toDate { get; set; }

        public async Task OnGetAsync() //On loading the page
        {
            var allAlbumsFromDB = from a in _db.Album.FromSqlRaw("SELECT * FROM dbo.Album"
                                                                + " WHERE CreateDate >= {0}"
                                                                + " AND CreateDate <= {1}", fromDate, toDate)
                                  select a;
            AllAlbums = await allAlbumsFromDB.ToListAsync();
            //fromDate = "This is my date";
        }
        public async Task OnPostAsync(string? to, string? from) //On clicking submit a form
        {
            fromDate = from;
            toDate = to;
            var allAlbumsFromDB = from a in _db.Album.FromSqlRaw("SELECT * FROM dbo.Album"
                                                               + " WHERE CreateDate >= {0}"
                                                               + " AND CreateDate <= {1}", fromDate, toDate)
                                  select a;
            var allSongsFromDB = from b in _db.Song.FromSqlRaw("SELECT * FROM dbo.Song"
                                                   + " WHERE UploadDate >= {0}"
                                                   + " AND UploadDate <= {1}", fromDate, toDate)
                                 select b;
            AllAlbums = await allAlbumsFromDB.ToListAsync();
            AllAlbumsSize = AllAlbums.Count;
            AllSongs = await allSongsFromDB.ToListAsync();
        }
    }
}