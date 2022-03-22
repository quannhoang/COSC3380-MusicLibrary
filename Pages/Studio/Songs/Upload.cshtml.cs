#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Studio.Songs
{
    public class UploadModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        private readonly BlobService _blobService;

        public UploadModel(MusicLibraryContext context, BlobService blobService)
        {
            _db = context;
            _blobService = blobService;
        }

        [BindProperty]
        public Song Song { get; set; }

        public IActionResult OnGet()
        {

            return Page();
        }

        [BindProperty]
        public IFormFile inputFile { get; set; }
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(IFormFile inputFile)
        {
            Song.FileName = "DummyName"; 
            if (!ModelState.IsValid) // Form inputs are invalid (ex: file name too short)
            {
                return Page();
            }
            if (inputFile == null || inputFile.Length < 1) return Page(); // Input file is empty or null

            var fileName = Guid.NewGuid() + Path.GetExtension(inputFile.FileName); // Generate new file name

            bool uploadSuccess = await _blobService.UploadFile(fileName.ToString(), inputFile); //Add SongID to filename ensure uniqueness
            if (uploadSuccess)
            {
                Song.Length = Song.Name.Length;
                Song.FileName = fileName;
                _db.Song.Add(Song);
                await _db.SaveChangesAsync();
            }
            // FIXME: Add Else print some error 

            return RedirectToPage("./Index");
        }
    }
}
