#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Browse.Songs
{
    public class PlaySongModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        private readonly BlobService _blobService;

        public PlaySongModel(MusicLibraryContext context, BlobService blobService)
        {
            _db = context;
            _blobService = blobService;
        }

        public Song Song { get; set; }

        [BindProperty]
        public string songUri { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Song = await _db.Song.FirstOrDefaultAsync(s => s.SongID == id);

            if (Song == null)
            {
                return NotFound();
            }
            songUri = await _blobService.GetFile(Song.FileName);
            return Page();
        }
    }
}
