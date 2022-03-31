using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MusicLibrary.Pages.Account
{
    public class LogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            await HttpContext.SignOutAsync("MusicLibraryCookie");
            return RedirectToPage("/Index");
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            await HttpContext.SignOutAsync("MusicLibraryCookie");
            return RedirectToPage("/Index");
        }
    }
}
