using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace MusicLibrary.Pages.Report
{
    public class IndexModel : PageModel
    {

        private readonly MusicLibraryContext _db; // Database object

        public IndexModel(MusicLibraryContext context) //Database object init
        {
            _db = context;
        }

        public IList<Album> AllAlBums { get; set; }

        public string fromDate { get; set; }

        public string toDate { get; set; }

        public async Task OnGetAsync() //On loading the page
        {
            var allAlbumsFromDB = from a in _db.Album.FromSqlRaw("SELECT * FROM dbo.Album"
                                                                + " WHERE CreateDate >= {0}"
                                                                + " AND CreateDate <= {1}", fromDate, toDate) select a;
            AllAlBums = await allAlbumsFromDB.ToListAsync();
            //fromDate = "This is my date";
        }   
        public void OnPost(string? to, string? from) //On clicking submit a form
        {
            if (to != null || from !=null)
            {
                Console.WriteLine(from);
                Console.WriteLine(to);
            }

        }
    }
}
