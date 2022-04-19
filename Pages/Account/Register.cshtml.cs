using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MusicLibrary.DataAccess.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MusicLibrary.Models;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;

namespace MusicLibrary.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly MusicLibraryContext _db;
        public RegisterModel(MusicLibraryContext context)
        {
            _db = context;
        }

        [BindProperty]
        public MusicLibrary.Models.User newUser { get; set; }

        //[BindProperty]
        public string errorMessage { get; set; }

        public IList<Email> EmailQueue { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            
            if (!ModelState.IsValid) // Check if all forms are filled correctly
            {
                return Page();
            }
            if (newUser.Passwords != newUser.PasswordsRetype)
            {
                errorMessage = "Passwords do not match";
                return Page();
            }
            if (UserExists(newUser.UserName))
            {
                errorMessage = "User name is already taken, please choose a different one";
                return Page();
            }
            byte[] salt = new byte[128 / 8]; //Salt for passwords hashing

            using (var rngCsp = new RNGCryptoServiceProvider()) // Generate random salt
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            // Hash passwords
            string HashedPass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                            password: newUser.Passwords,
                                                            salt: salt,
                                                            prf: KeyDerivationPrf.HMACSHA256,
                                                            iterationCount: 100000,
                                                            numBytesRequested: 256 / 8));
            // Add new user to DB
            newUser.Passwords = HashedPass;
            newUser.Salt = Convert.ToBase64String(salt);
            _db.User.Add(newUser);
            await _db.SaveChangesAsync();

            // Process EmailQueue
            var emailQueue = from e in _db.EmailQueue.FromSqlRaw("SELECT * FROM [dbo].[EmailQueue] WHERE SentTime is NULL")
                             select e;
            EmailQueue = emailQueue.ToList();
            foreach (var email in EmailQueue)
            {
                Console.WriteLine(email.Recipients + ' ' + email.Email_Subject);
                if (SendEmail(email) == true) updateSentTime(email);
            }

            return RedirectToPage("./Login");

        }

        public bool UserExists(string inputUserName)
        {
            return _db.User.Any(u => u.UserName == inputUserName);
        }
        private bool SendEmail(Email inputEmail)
        {

            string to = inputEmail.Recipients;
            string from = "mus1cdbcosc3380adm1n@gmail.com";
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
