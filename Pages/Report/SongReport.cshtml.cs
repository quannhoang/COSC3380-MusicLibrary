using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Report
{
    public class SongReportModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public SongReportModel(MusicLibraryContext context)
        {
            _db = context;
        }

        public IList<string> BarChartItems { get; set; } = new List<string>( new string[]{});
        public IList<int> BarChartData { get; set; } = new List<int>( new int[]{});

        public IList<string> LineChartItems { get; set; } = new List<string>(new string[] { });
        public IList<int> LineChartData { get; set; } = new List<int>(new int[] { });

        public IList<Song> SongList { get; set; }

        public IList<View>  ViewList { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync(string? from, string? to)
        {
            if (from == null || to == null) // If either from or to date is not provided, does nothing
            {
                return Page();
            }

            if (DateTime.Parse(to).Date < DateTime.Parse(from).Date)
            {
                ErrorMessage = "From-Date cannot be later than To-Date";
            }

            var songs = from s in _db.Song.FromSqlRaw($"SELECT * FROM [dbo].[Song] WHERE UploadDate >= '{from}' AND UploadDate <= '{to}'")
                        select s ;
            SongList = await songs.ToListAsync();
            for (var day = DateTime.Parse(from).Date; day.Date <= DateTime.Parse(to).Date; day = day.AddDays(1))
            {
                int dateSongCount = SongList.Count(s => s.UploadDate == day);
                string date = day.ToShortDateString();
                BarChartData.Add(dateSongCount);
                BarChartItems.Add(date);
            }

            var views = from v in _db.View.FromSqlRaw($"SELECT * FROM [dbo].[View] WHERE ViewDate >= '{from}' AND ViewDate <= '{to}'")
                        select v;
            ViewList = await views.ToListAsync();
            for (var day = DateTime.Parse(from).Date; day.Date <= DateTime.Parse(to).Date; day = day.AddDays(1))
            {
                int dateViewCount = ViewList.Count(s => s.ViewDate.Date == day);
                string date = day.ToShortDateString();
                LineChartData.Add(dateViewCount);
                LineChartItems.Add(date);
            }
            return Page();
        }
    }
}
