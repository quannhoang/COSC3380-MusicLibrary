using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;

namespace MusicLibrary.Pages.Account
{
    [Authorize(Policy = "MustBeAdmin")]
    public class ManageModel : PageModel
    {
        private readonly MusicLibraryContext _db;

        public ManageModel(MusicLibraryContext context)
        {
            _db = context;
        }
        public IList<MusicLibrary.Models.User> Users { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchString { get; set; }

        public async Task OnGet()
        {
            // Select all users from DB
            var allusers = from u in _db.User select u;

            // Search for username if searchString is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                allusers = allusers.Where(u => u.UserName.Contains(searchString));
            }
            Users = await allusers.ToListAsync();
        }
    }
}
