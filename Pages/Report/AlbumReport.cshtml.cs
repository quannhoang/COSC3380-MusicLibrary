using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MusicLibrary.Pages.Report
{
    [Authorize]
    public class AlbumReportModel : PageModel
    {

        private readonly MusicLibraryContext _db; // Database object

        public AlbumReportModel(MusicLibraryContext context) //Database object init
        {
            _db = context;
        }

        public bool Submitted { get; set; } = false;

        public string ErrorMessage { get; set; } = string.Empty;

        public IList<Album> AllAlbums { get; set; }
        public int AllAlbumsSize { get; set; } = 0;
        public IList<Song> AllSongs { get; set; }

        public string fromDate { get; set; }

        public string toDate { get; set; }

        public IList<ReportItem> ReportItemList { get; set; } = new List<ReportItem>(new ReportItem[] { });
        public IList<string> BarChartItems { get; set; } = new List<string>(new string[] { });
        public IList<int> BarChartData { get; set; } = new List<int>(new int[] { });

        public async Task OnGetAsync() //On loading the page
        {
            var allAlbumsFromDB = from a in _db.Album.FromSqlRaw("SELECT * FROM dbo.Album"
                                                                + " WHERE CreateDate >= {0}"
                                                                + " AND CreateDate <= {1}", fromDate, toDate)
                                  select a;
            AllAlbums = await allAlbumsFromDB.ToListAsync();
            //fromDate = "This is my date";
        }
        public async Task<IActionResult> OnPostAsync(string? to, string? from) //On clicking submit a form
        {
            // If from date is later than to date, display error 
            if (DateTime.Parse(to).Date < DateTime.Parse(from).Date)
            {
                ErrorMessage = "From-Date cannot be later than To-Date";
                return Page();
            }

            fromDate = from;
            toDate = to;

            Submitted = true;
            var allAlbumsFromDB = from a in _db.Album.FromSqlRaw("SELECT * FROM dbo.Album"
                                                               + " WHERE CreateDate >= {0}"
                                                               + " AND CreateDate <= {1}", fromDate, toDate)
                                  select a;
            var artists = from a in allAlbumsFromDB select a.ArtistName;
            
            AllAlbums = await allAlbumsFromDB.ToListAsync();
            AllAlbumsSize = AllAlbums.Count;

            // Calculate data for reports and Bar chart
            ReportItem albumCount = new ReportItem();
            albumCount.Label = "Total albums created";
            albumCount.Value = AllAlbumsSize.ToString();
            ReportItemList.Add(albumCount);

            ReportItem artistCount = new ReportItem();
            artistCount.Label = "Number of artists";
            artistCount.Value = artists.Distinct().Count().ToString();
            ReportItemList.Add(artistCount);

            
            for (var day = DateTime.Parse(from).Date; day.Date <= DateTime.Parse(to).Date; day = day.AddDays(1))
            {
                int dateAlbumCount = AllAlbums.Count(a => a.CreateDate == day);
                string date = day.ToShortDateString();
                

                BarChartData.Add(dateAlbumCount);
                BarChartItems.Add(date);
            }

            int totalSongCount = 0;
            foreach (var album in AllAlbums)
            {
                totalSongCount += album.SongCount;
            }

            ReportItem songCount = new ReportItem();
            songCount.Label = "Total songs";
            songCount.Value = totalSongCount.ToString();
            ReportItemList.Add(songCount);

            ReportItem avgSongPerAlbum = new ReportItem();
            avgSongPerAlbum.Label = "Average number of songs per album";
            avgSongPerAlbum.Value = ((float)totalSongCount/AllAlbums.Count).ToString();
            ReportItemList.Add(avgSongPerAlbum);

            return Page();
        }
    }
}