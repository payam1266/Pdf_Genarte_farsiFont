using ChopSuey.Areas.Identity.Data;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChopSuey.Services
{
    public class AllUserPdfGenerator:BaseUserPdfGenerator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AllUserPdfGenerator(UserManager<ApplicationUser> userManager):base(userManager)
        {
           _userManager = userManager;
        }
        public override async Task<PdfPTable> GenerateUserPdfTable()
        {
            var Users =await  _userManager.Users.ToListAsync();

            var table = CreatePdfTable();
            AddHeaderRow(table);

            foreach (var user in Users)
            {
                await AddUserRow(table, user);
            }
            return table;
        }
    }
}
