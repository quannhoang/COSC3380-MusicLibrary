#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;
using Microsoft.AspNetCore.Authorization;

namespace MusicLibrary.Pages.Studio.Songs
{
    [Authorize]
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
        public string loggedInUserName { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            loggedInUserName = HttpContext.User.Identity.Name;
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
            _db.Database.ExecuteSqlRaw("Insert into [dbo].[View] (UserName, SongID, ViewDate) values({0}, {1}, {2}) ", loggedInUserName, id, DateTime.Now);
            return Page();
        }
    }
}
