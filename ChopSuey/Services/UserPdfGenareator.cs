using ChopSuey.Areas.Identity.Data;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Identity;

namespace ChopSuey.Services
{
    public class UserPdfGenareator:BaseUserPdfGenerator
    {
        public UserPdfGenareator(UserManager<ApplicationUser> userManager):base(userManager)
        {
            
        }
        public async Task<PdfPTable> GenerateUserPdfTable(ApplicationUser user)
        {
            var table = CreatePdfTable();
            AddHeaderRow(table);
            await AddUserRow(table, user);
            return table;
        }
    }
}
