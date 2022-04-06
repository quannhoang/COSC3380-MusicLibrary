using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;

namespace MusicLibrary.Pages.Account
{
    [Authorize(Policy = "MustBeAdmin")]
    public class UnsuspendModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public UnsuspendModel(MusicLibraryContext context)
        {
            _db = context;
        }

        [BindProperty]
        public MusicLibrary.Models.User CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CurrentUser = await _db.User.FirstOrDefaultAsync(u => u.UserID == id);

            if (CurrentUser == null)
            {
                return NotFound();
            }

            if (!CurrentUser.IsSuspended || CurrentUser.UserName == User.Identity.Name)
            {
                return RedirectToPage("./Manage");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            CurrentUser = await _db.User.FirstOrDefaultAsync(u => u.UserID == id);

            if (CurrentUser != null)
            {
                CurrentUser.IsSuspended = false;
                await _db.SaveChangesAsync();
            }


            return RedirectToPage("./Manage");
        }
    }
}
