namespace MusicLibrary.Pages.Report
{
    public class ArtistProfileModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public ArtistProfileModel(MusicLibraryContext context)
        {
            _db = context;
        }

        public SelectList ArtistList { get; set; }
        public IList<MusicLibrary.Models.User> CurrentArtist { get; set; }

        [BindProperty]
        public string inputArtistName { get; set; } = string.Empty;
        public IList<Song> SongList { get; set; }
        public IList<Album> AlbumList { get; set; }
        public IList<View> ViewList { get; set; }
        public IList<Like> LikeList { get; set; }
        public bool Submitted = false;
        public IList<ReportItem> ReportItemList { get; set; } = new List<ReportItem>(new ReportItem[] { });
        public IList<string> BarChartItems { get; set; } = new List<string>(new string[] { });
        public IList<int> BarChartData { get; set; } = new List<int>(new int[] { });

        public IList<string> PieChartItems { get; set; } = new List<string>(new string[] { });
        public IList<int> PieChartData { get; set; } = new List<int>(new int[] { });

        public async Task<IActionResult> OnGetAsync()
        {
            // When loading the page, get all artist names for selection box in front end
            IQueryable<string> allArtist = from u in _db.User.FromSqlRaw("SELECT * FROM [dbo].[User] WHERE isArtist=1")
                                           orderby u.UserName
                                           select u.UserName;
            ArtistList = new SelectList(await allArtist.ToListAsync());
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // If no artist name provided, return to Page
            if (inputArtistName == null) return await OnGetAsync();

            // If artist name is not found in DB
            var artist = _db.User.FirstOrDefault(u => u.UserName == inputArtistName);
            if (artist == null) return await OnGetAsync();

            ReportItem joinDate = new ReportItem();
            joinDate.Label = "Date joined";
            joinDate.Value = artist.CreateDate.ToShortDateString();
            ReportItemList.Add(joinDate);

            // Get all artist names for selection box in front end
            var Artist = from u in _db.User.FromSqlRaw($"SELECT * FROM [dbo].[User] WHERE isArtist=1 AND UserName = {inputArtistName}")
                         select u;
            CurrentArtist = await Artist.ToListAsync();

            Submitted = true;
            // Get all songs uploaded by inputArtistName
            var songs = from s in _db.Song.FromSqlRaw("SELECT * FROM [dbo].[Song] WHERE Artist={0}", inputArtistName)
                        select s;
            SongList = await songs.ToListAsync();

            ReportItem totalSong = new ReportItem();
            totalSong.Label = "Songs Upload";
            totalSong.Value = SongList.Count().ToString();
            ReportItemList.Add(totalSong);

            //Get all albums created by inputArtistName
            var albums = from a in _db.Album.FromSqlRaw("SELECT * FROM [dbo].[Album] WHERE ArtistName={0}", inputArtistName)
                         select a;
            AlbumList = await albums.ToListAsync();

            ReportItem totalAlbum = new ReportItem();
            totalAlbum.Label = "Albums Created";
            totalAlbum.Value = AlbumList.Count().ToString();
            ReportItemList.Add(totalAlbum);

            // Get all likes for inputArtistName
            var likes = from l in _db.Like.FromSqlRaw("SELECT L.* FROM [dbo].[Like] as L, [dbo].[Song] as S" +
                                                        " WHERE L.SongID = S.SongID" +
                                                        " AND S.Artist = {0}", inputArtistName)
                        select l;
            LikeList = await likes.ToListAsync();

            ReportItem totaLike = new ReportItem();
            totaLike.Label = "Likes Received";
            totaLike.Value = LikeList.Count().ToString();
            ReportItemList.Add(totaLike);

            // Get all views for inputArtistName
            var views = from v in _db.View.FromSqlRaw("SELECT V.* FROM [dbo].[View] as V, [dbo].[Song] as S" +
                                            " WHERE V.SongID = S.SongID" +
                                            " AND S.Artist = {0}", inputArtistName)
                        select v;
            ViewList = await views.ToListAsync();

            ReportItem totalView = new ReportItem();
            totalView.Label = "Views Received";
            totalView.Value = ViewList.Count().ToString();
            ReportItemList.Add(totalView);

            var maxLikeCount = -1;
            var mostLikedSongName = string.Empty;
            foreach (var song in SongList)
            {
                if (song.LikeCount > maxLikeCount)
                {
                    maxLikeCount = song.LikeCount;
                    mostLikedSongName = song.Name;
                }
            }

            ReportItem mostLikedSong = new ReportItem();
            mostLikedSong.Label = "Most liked song";
            mostLikedSong.Value = mostLikedSongName;
            ReportItemList.Add(mostLikedSong);


            var maxViewCount = -1;
            var mostViewedSongName = string.Empty;
            foreach (var song in SongList)
            {
                if (song.ViewCount > maxViewCount)
                {
                    maxViewCount = song.ViewCount;
                    mostViewedSongName = song.Name;
                }
            }

            ReportItem mostViewedSong = new ReportItem();
            mostViewedSong.Label = "Most viewed song";
            mostViewedSong.Value = mostViewedSongName;
            ReportItemList.Add(mostViewedSong);

            // Get all song genres uploaded by inputArtistName and calculate data for Pie Chart
            var genres = from s in _db.Song.FromSqlRaw("SELECT * FROM [dbo].[Song] WHERE Artist={0}", inputArtistName)
                         orderby s.Genre
                         select s.Genre;
            PieChartItems = new List<string>(await genres.Distinct().ToListAsync());

            foreach (var genre in PieChartItems)
            {
                PieChartData.Add(SongList.Count(s => s.Genre == genre));
            }

            // Calculate data for BarChart
            for (var i = 0; i < 5; i++)
            {
                var currentMaxView = SongList.Max(s => s.ViewCount);
                var currentMaxViewSongName = SongList.First(s => s.ViewCount == currentMaxView).Name;
                var currentMaxViewSongID = SongList.First(s => s.ViewCount == currentMaxView).SongID;
                BarChartItems.Add(currentMaxViewSongName);
                BarChartData.Add(currentMaxView);
                songs = from s in songs where s.SongID != currentMaxViewSongID select s;
                SongList = await songs.ToListAsync();
                if (SongList.Count() == 0) break;
            }



            return Page();
        }
    }
}
