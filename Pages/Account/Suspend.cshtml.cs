using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;

namespace MusicLibrary.Pages.Account
{
    [Authorize(Policy = "MustBeAdmin")]
    public class SuspendModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public SuspendModel(MusicLibraryContext context)
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

            if (CurrentUser.IsSuspended || CurrentUser.IsAdmin || CurrentUser.UserName == User.Identity.Name)
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
                CurrentUser.IsSuspended = true;
                await _db.SaveChangesAsync();
                _db.Database.ExecuteSqlRaw("INSERT INTO [dbo].[SuspendHistory] (UserName, Email, SuspendDate)" +
                                " values({0}, {1}, getdate())", CurrentUser.UserName, CurrentUser.Email);
            }
            
          

            return RedirectToPage("./Manage");
        }
    }
}
