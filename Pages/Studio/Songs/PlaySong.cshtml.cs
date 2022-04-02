﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;

namespace MusicLibrary.Pages.Studio.Songs
{
    public class PlaySongModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        private readonly BlobService _blobService;

        private readonly IHostEnvironment _hostingEnvironment;

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

            Song = _db.Song.FromSqlRaw("SELECT * from dbo.Song WHERE SongID = {0}", id).FirstOrDefault();
            
            if (Song == null)
            {
                return NotFound();
            }
            songUri = await _blobService.GetFile(Song.FileName);

            return Page();
        }
    }
}
