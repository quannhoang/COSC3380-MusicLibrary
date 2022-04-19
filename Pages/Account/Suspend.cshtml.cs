using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicLibrary.DataAccess.Data;
using MusicLibrary.Models;
using System.Net;
using System.Net.Mail;

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

        public IList<Email> EmailQueue { get; set; }

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
                // Suspend user
                CurrentUser.IsSuspended = true;
                await _db.SaveChangesAsync();
                _db.Database.ExecuteSqlRaw("INSERT INTO [dbo].[SuspendHistory] (UserName, Email, SuspendDate)" +
                                " values({0}, {1}, getdate())", CurrentUser.UserName, CurrentUser.Email);

                // Process email queue
                var emailQueue = from e in _db.EmailQueue.FromSqlRaw("SELECT * FROM [dbo].[EmailQueue] WHERE SentTime is NULL")
                                 select e;
                EmailQueue = emailQueue.ToList();
                foreach (var email in EmailQueue)
                {
                    Console.WriteLine(email.Recipients + ' ' + email.Email_Subject);
                    if (SendEmail(email)==true) updateSentTime(email);
                }
            }
            
          

            return RedirectToPage("./Manage");
        }
        private bool SendEmail(Email inputEmail)
        {

            string to = inputEmail.Recipients;
            string from = "mus1cdbcosc3380adm1n@gmail.com";
            try
            {
                using (MailMessage mailMessage = new MailMessage(from, to))
                {
                    mailMessage.Subject = inputEmail.Email_Subject;
                    mailMessage.Body = inputEmail.Email_body;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential NetworkCredential = new NetworkCredential(from, "Cosc3380@");
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = NetworkCredential;
                    smtp.Port = 587;
                    mailMessage.IsBodyHtml = true;
                    try
                    {
                        smtp.Send(mailMessage);
                    }

                    catch (Exception ex)
                    {
                        return false;
                    }

                }
            }
            catch (Exception ex)
            {
                return false;
            }
            

            return true;

        }
        
        private void updateSentTime(Email inputEmail)
        {
            var email = _db.EmailQueue.FirstOrDefault(e => e.Id == inputEmail.Id);
            if (email != null && email.SentTime == null)
            {
                _db.Database.ExecuteSqlRaw("UPDATE [dbo].[EmailQueue] SET SentTime={0} WHERE Id={1}", DateTime.Now, email.Id);
            }
        }
    }
}
