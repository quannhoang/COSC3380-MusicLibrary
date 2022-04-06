using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace MusicLibrary.Pages.Browse.Albums
{
   [Authorize]
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
