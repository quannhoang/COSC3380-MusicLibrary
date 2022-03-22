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

namespace MusicLibrary.Pages.Studio.Playlists
{
    public class DeleteModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        private readonly BlobService _blobService;
        public DeleteModel(MusicLibraryContext context, BlobService blobService)
        {
            _db = context;
            _blobService = blobService;
        }

        [BindProperty]
        public Song Song { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Song = await _db.Song.FirstOrDefaultAsync(m => m.SongID == id);

            if (Song == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Song = await _db.Song.FindAsync(id);

            if (Song != null)
            {
                bool deleteSuccess = await _blobService.DeleteFile(Song.FileName);
                if (deleteSuccess)
                {
                    //TODO: Delete song's records in AlbumSongs and PLaylistSongs before delete Song
                    _db.Song.Remove(Song);
                    await _db.SaveChangesAsync();
                }
                else { } //TODO: If delete failed on Azlure blob storage, inform user with an error message
            }

            return RedirectToPage("./Index");
        }
    }
}
