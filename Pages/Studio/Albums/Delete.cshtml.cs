﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace MusicLibrary.Pages.Studio.Albums
{
    [Authorize]
	public class DeleteModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
