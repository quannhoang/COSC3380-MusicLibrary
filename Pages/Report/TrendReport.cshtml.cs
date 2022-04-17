using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Report
{
    public class TrendReportModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public TrendReportModel(MusicLibraryContext context)
        {
            _db = context;
        }

        public bool submitted { get; set; } = false;

        public string frontFrom { get; set; } = string.Empty;

        public string frontTo { get; set; } = string.Empty;

        public IList<ReportItem> ReportItemList { get; set; } = new List<ReportItem>(new ReportItem[] { });

        public IList<string> BarChartItems { get; set; } = new List<string>(new string[] { });
        public IList<int> BarChartData { get; set; } = new List<int>(new int[] { });

        public IList<string> LineChartItems { get; set; } = new List<string>(new string[] { });
        public IList<int> LineChartData { get; set; } = new List<int>(new int[] { });

        public IList<Song> SongList { get; set; }

        public IList<View> ViewList { get; set; }
        public Dictionary<string, int> GenreViewList = new Dictionary<string, int>();

        public string ErrorMessage { get; set; } = string.Empty;

        public void OnGet()
        {

        }
        public async Task<IActionResult> OnPostAsync(string? from, string? to)
        {
            // If either from or to date is not provided, does nothing
            if (from == null || to == null)
            {
                return Page();
            }

            // If from date is later than to date, display error 
            if (DateTime.Parse(to).Date < DateTime.Parse(from).Date)
            {
                ErrorMessage = "From-Date cannot be later than To-Date";
                return Page();
            }

            // Assign frontFrom and frontTo for front end to display
            frontFrom = from;
            frontTo = to;

            // Toggle submitted for front end to display results
            submitted = true;

            // Get all songs uploaded between from and to dates
            var songs = from s in _db.Song.FromSqlRaw($"SELECT * FROM [dbo].[Song] WHERE UploadDate >= '{from}' AND UploadDate <= '{to}'")
                        select s;
            SongList = await songs.ToListAsync();

            // Do calculation for song upload trend report and chart

            string maxDate = String.Empty;
            int maxCount = 0;
            for (var day = DateTime.Parse(from).Date; day.Date <= DateTime.Parse(to).Date; day = day.AddDays(1))
            {
                int dateSongCount = SongList.Count(s => s.UploadDate == day);
                string date = day.ToShortDateString();
                if (dateSongCount > maxCount)
                {
                    maxCount = dateSongCount;
                    maxDate = date;
                }
                BarChartData.Add(dateSongCount);
                BarChartItems.Add(date);
            }

            ReportItem totalSUpload = new ReportItem();
            totalSUpload.Label = "Total song uploads";
            totalSUpload.Value = BarChartData.Sum().ToString();
            ReportItemList.Add(totalSUpload);

            ReportItem avgSUpload = new ReportItem();
            avgSUpload.Label = "Average daily uploads";
            avgSUpload.Value = ((float)BarChartData.Sum() / BarChartData.Count()).ToString();
            ReportItemList.Add(avgSUpload);

            ReportItem minSUpload = new ReportItem();
            minSUpload.Label = "Mininum daily uploads";
            minSUpload.Value = BarChartData.Min().ToString();
            ReportItemList.Add(minSUpload);

            ReportItem maxSUpload = new ReportItem();
            maxSUpload.Label = "Maximum daily uploads";
            maxSUpload.Value = BarChartData.Max().ToString();
            ReportItemList.Add(maxSUpload);

            ReportItem maxSUploadDate = new ReportItem();
            maxSUploadDate.Label = "Date that has maximum uploads";
            maxSUploadDate.Value = maxDate;
            ReportItemList.Add(maxSUploadDate);


            // Get the most popular song and genre between from and to dates
            Song maxView = SongList[0];
            foreach (Song song in SongList)
            {
                if (song.ViewCount + song.LikeCount > maxView.ViewCount + maxView.LikeCount)
                {
                    maxView = song;
                }

                if (!GenreViewList.ContainsKey(song.Genre))
                {
                    GenreViewList[song.Genre] = song.ViewCount;
                }
                else
                {
                    GenreViewList[song.Genre] += song.ViewCount;
                }
            }

            int GenrePopular = GenreViewList.Values.Max();

            ReportItem mostPopularSong = new ReportItem();
            mostPopularSong.Label = "The most popular song";
            mostPopularSong.Value = maxView.Name;
            ReportItemList.Add(mostPopularSong);

            ReportItem mostPopularGenre = new ReportItem();
            mostPopularGenre.Label = "Most popular genre";
            mostPopularGenre.Value = GenreViewList.FirstOrDefault(x => x.Value == GenrePopular).Key;
            ReportItemList.Add(mostPopularGenre);

            // Get all views that have Viewdate between from and to dates
            var views = from v in _db.View.FromSqlRaw($"SELECT * FROM [dbo].[View] WHERE ViewDate >= '{from}' AND ViewDate <= '{to}'")
                        select v;
            ViewList = await views.ToListAsync();

            // Do calculation for view trend report and line chart
            maxDate = String.Empty;
            maxCount = 0;
            for (var day = DateTime.Parse(from).Date; day.Date <= DateTime.Parse(to).Date; day = day.AddDays(1))
            {
                int dateViewCount = ViewList.Count(s => s.ViewDate.Date == day);
                string date = day.ToShortDateString();
                if (dateViewCount > maxCount)
                {
                    maxCount = dateViewCount;
                    maxDate = date;
                }
                LineChartData.Add(dateViewCount);
                LineChartItems.Add(date);
            }

            ReportItem totalVUpload = new ReportItem();
            totalVUpload.Label = "Total Views";
            totalVUpload.Value = LineChartData.Sum().ToString();
            ReportItemList.Add(totalVUpload);

            ReportItem avgVUpload = new ReportItem();
            avgVUpload.Label = "Average daily Views";
            avgVUpload.Value = ((float)LineChartData.Sum() / LineChartData.Count()).ToString();
            ReportItemList.Add(avgVUpload);

            ReportItem minVUpload = new ReportItem();
            minVUpload.Label = "Mininum daily Views";
            minVUpload.Value = LineChartData.Min().ToString();
            ReportItemList.Add(minVUpload);

            ReportItem maxVUpload = new ReportItem();
            maxVUpload.Label = "Maximum daily Views";
            maxVUpload.Value = LineChartData.Max().ToString();
            ReportItemList.Add(maxVUpload);

            ReportItem maxVUploadDate = new ReportItem();
            maxVUploadDate.Label = "Date that has maximum Views";
            maxVUploadDate.Value = maxDate;
            ReportItemList.Add(maxVUploadDate);

            return Page();
        }
    }
}
