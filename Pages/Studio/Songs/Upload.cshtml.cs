#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
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

        private readonly IHostEnvironment _hostingEnvironment;

        public UploadModel(MusicLibraryContext context, BlobService blobService, IHostEnvironment hostingEnvironment)
        {
            _db = context;
            _blobService = blobService;
            _hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public Song Song { get; set; }

        public string loggedInUserName { get; set; } = string.Empty;

        public List<string> allowedFiles { get; set; } = new List<string> {".m4a", ".mp3", ".flac", ".mp4", ".wav", ".wma", ".aac" };

        public string fileNotAllowedMessage { get; set; } = string.Empty;

        public IActionResult OnGet()
        {

            return Page();
        }

        [BindProperty]
        public IFormFile inputFile { get; set; }
        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(IFormFile inputFile)
        {
            loggedInUserName = HttpContext.User.Identity.Name;
            if (!ModelState.IsValid) // Form inputs are invalid (ex: file name too short)
            {
                return Page();
            }
            if (inputFile == null || inputFile.Length < 1) return Page(); // Input file is empty or null
            // Only allow files ".m4a, .mp3, .flac, .mp4, .wav, .wma, .aac"
            if (!allowedFiles.Contains(Path.GetExtension(inputFile.FileName)))
            {
                fileNotAllowedMessage = "Only files M4A, MP3, MP4, FLAC, WAV, WMA and AAC are allowed";
                return Page();
            }

            // Generate new file name
            var fileName = Guid.NewGuid() + Path.GetExtension(inputFile.FileName); 

            // Use Taglib library to calculate audio duration
            var serverfilePath = _hostingEnvironment.ContentRootPath + "\\wwwroot\\" + fileName;
            using (Stream fileStream = new FileStream(serverfilePath, FileMode.Create))
            {
                await inputFile.CopyToAsync(fileStream);
            }
            // Get duration
            var tfile = TagLib.File.Create(serverfilePath);
            var duration = string.Format("{0}:{1}:{2}", 
                tfile.Properties.Duration.Hours.ToString("D2"), 
                tfile.Properties.Duration.Minutes.ToString("D2"), 
                tfile.Properties.Duration.Seconds.ToString("D2"));
            // Delete temp file from server
            System.IO.File.Delete(serverfilePath);

            bool uploadSuccess = await _blobService.UploadFile(fileName.ToString(), inputFile); 
            if (uploadSuccess)
            {
                Song.Artist = loggedInUserName;
                Song.Length = duration;
                Song.FileName = fileName;
                _db.Song.Add(Song);
                await _db.SaveChangesAsync();
            }
            // FIXME: Add Else print some error 

            return RedirectToPage("./Index");
        }
    }
}
