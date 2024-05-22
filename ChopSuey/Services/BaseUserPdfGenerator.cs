using iTextSharp.text.pdf;
using iTextSharp.text;
using ChopSuey.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ChopSuey.Services
{
    public class BaseUserPdfGenerator
    {
        protected Font _fontIranSanse;
        protected Font _fontShabnam;
        private readonly UserManager<ApplicationUser> _userManager;

        public BaseUserPdfGenerator(UserManager<ApplicationUser> userManager)
        {
            string fontPathIranSanse = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fonts/iransansxvf.ttf");
            string fontPathShabnam = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fonts/shabnam-bold-fd.ttf");
            BaseFont baseFontIransanse = BaseFont.CreateFont(fontPathIranSanse, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            BaseFont baseFontshabnam = BaseFont.CreateFont(fontPathShabnam, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            _fontIranSanse = new Font(baseFontIransanse, 10, Font.BOLD, BaseColor.BLACK);
            _fontShabnam = new Font(baseFontshabnam, 10, Font.BOLD, BaseColor.WHITE);
            _userManager = userManager;
        }
        public virtual async Task<PdfPTable> GenerateUserPdfTable()
        {
            PdfPTable table = CreatePdfTable();

            AddHeaderRow(table);
            await Task.CompletedTask;
            return table;
        }
        protected virtual PdfPTable CreatePdfTable()
        {
            PdfPTable table = new PdfPTable(7);
            float[] columnWidths = new float[] { 2.5f, 2f, 3f, 1.5f, 3.5f, 2.5f, 2f };
            table.SetWidths(columnWidths);
            table.WidthPercentage = 100;
            table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
            return table;
        }

        protected virtual void AddHeaderRow(PdfPTable table)
        {
            AddHeaderCell(table, "نام");
            AddHeaderCell(table, "نام خانوادگی");
            AddHeaderCell(table, "ایمیل");
            AddHeaderCell(table, "سن");
            AddHeaderCell(table, "تلفن");
            AddHeaderCell(table, "شهر");
            AddHeaderCell(table, "نقش");
        }
        protected virtual async Task AddUserRow(PdfPTable table, ApplicationUser user)
        {
            AddCell(table, user.firstName);
            AddCell(table, user.lastName);
            AddCell(table, user.Email);
            AddCell(table, user.age.ToString());
            AddCell(table, user.PhoneNumber);
            AddCell(table, user.city);
            var roles = await _userManager.GetRolesAsync(user);
            string rolesString = string.Join(", ", roles);
            AddCell(table, rolesString);
        }

        protected virtual void AddHeaderCell(PdfPTable table, string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, _fontShabnam));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 10;
            cell.BackgroundColor = BaseColor.DARK_GRAY;
            table.AddCell(cell);
        }

        protected virtual void AddCell(PdfPTable table, string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, _fontIranSanse));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 10;
            table.AddCell(cell);
        }
    }
}
