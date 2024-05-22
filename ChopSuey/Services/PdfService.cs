using ChopSuey.Areas.Identity.Data;
using ChopSuey.Contracts;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Path = System.IO.Path;

namespace ChopSuey.Services
{
    public class PdfService : IPdfService
    {
        private readonly UserManager<ApplicationUser> _userManager;
       
        public PdfService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<byte[]> CreatePdfAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            string rolesString = string.Join(", ", roles);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      
            MemoryStream ms = new MemoryStream();
            using (Document document = new Document(PageSize.A4))
            {
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                string fontPathIranSanse = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fonts/iransansxvf.ttf");
                string fontPathShabnam = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fonts/shabnam-bold-fd.ttf");
                BaseFont baseFontIransanse = BaseFont.CreateFont(fontPathIranSanse, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                BaseFont baseFontshabnam = BaseFont.CreateFont(fontPathShabnam, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                Font fontIranSanse = new Font(baseFontIransanse, 10, Font.BOLD, BaseColor.BLACK);
                Font fontShabnam = new Font(baseFontshabnam, 10, Font.BOLD, BaseColor.WHITE);

                PdfPTable title = new PdfPTable(1);

                title.WidthPercentage = 100;

                title.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                PdfPCell celltitel1 = new PdfPCell(new Phrase($"تاریخ : {DateTime.Now:yyyy-MM-dd} ", fontIranSanse));
                celltitel1.HorizontalAlignment = Element.ALIGN_LEFT;
                celltitel1.Padding = 5;
                celltitel1.BorderColor = BaseColor.WHITE;
                title.AddCell(celltitel1);
                PdfPCell celltitel2 = new PdfPCell(new Phrase("عنوان : اطلاعات کاربری", fontIranSanse));
                celltitel2.HorizontalAlignment = Element.ALIGN_LEFT;
                celltitel2.Padding = 5;
                celltitel2.PaddingBottom = 15;
                celltitel2.BorderColor = BaseColor.WHITE;
                title.AddCell(celltitel2);

                PdfPTable table = new PdfPTable(7);

                float[] columnWidths = new float[] { 2.5f, 2f, 3f, 1.5f, 3.5f, 2.5f, 2f };
                table.SetWidths(columnWidths);
                table.WidthPercentage = 100;
                table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                AddHeederTabble(table, fontShabnam, "نام");
                AddHeederTabble(table, fontShabnam, "نام خانوادگی");
                AddHeederTabble(table, fontShabnam, "ایمیل");
                AddHeederTabble(table, fontShabnam, "سن");
                AddHeederTabble(table, fontShabnam, "تلفن");
                AddHeederTabble(table, fontShabnam, "شهر");
                AddHeederTabble(table, fontShabnam, "نقش");

                AddCellTable(table, fontIranSanse, user.firstName);
                AddCellTable(table, fontIranSanse, user.lastName);
                AddCellTable(table, fontIranSanse, user.Email);
                AddCellTable(table, fontIranSanse, user.age.ToString());
                AddCellTable(table, fontIranSanse, user.PhoneNumber);
                AddCellTable(table, fontIranSanse, user.city);
                AddCellTable(table, fontIranSanse, rolesString);

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Pictuere/Screenshot 2023-09-30 111644.png");
                Image logo = Image.GetInstance(logoPath);
                logo.ScaleToFit(150, 150);
                logo.Alignment = Image.ALIGN_CENTER;
                logo.SetAbsolutePosition((document.PageSize.Width - logo.ScaledWidth) / 2, document.BottomMargin / 2);

                document.Add(title);
                document.Add(table);
                document.Add(logo);
                document.Close();
            }

            return ms.ToArray();
        }

        public async Task<byte[]> GenerateUserListPdfAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
         
            MemoryStream ms = new MemoryStream();
            using (Document document = new Document(PageSize.A4))
            {
                PdfWriter writer = PdfWriter.GetInstance(document, ms);

                document.Open();

                string fontPathIranSanse = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fonts/iransansxvf.ttf");
                string fontPathShabnam = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fonts/shabnam-bold-fd.ttf");
                BaseFont baseFontIransanse = BaseFont.CreateFont(fontPathIranSanse, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                BaseFont baseFontshabnam = BaseFont.CreateFont(fontPathShabnam, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                Font fontIranSanse = new Font(baseFontIransanse, 10, Font.BOLD, BaseColor.BLACK);
                Font fontShabnam = new Font(baseFontshabnam, 10, Font.BOLD, BaseColor.WHITE);

                PdfPTable title = new PdfPTable(1);

                title.WidthPercentage = 100; 

                title.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                PdfPCell celltitel1 = new PdfPCell(new Phrase($"تاریخ : {DateTime.Now:yyyy-MM-dd} ", fontIranSanse));
                celltitel1.HorizontalAlignment = Element.ALIGN_LEFT;
                celltitel1.Padding = 5;
                celltitel1.BorderColor = BaseColor.WHITE;
                title.AddCell(celltitel1);
                PdfPCell celltitel2 = new PdfPCell(new Phrase("عنوان : اطلاعات کاربری", fontIranSanse));
                celltitel2.HorizontalAlignment = Element.ALIGN_LEFT;
                celltitel2.Padding = 5;
                celltitel2.PaddingBottom = 15;
                celltitel2.BorderColor = BaseColor.WHITE;
                title.AddCell(celltitel2);

                PdfPTable table = new PdfPTable(7);

                float[] columnWidths = new float[] { 2.5f, 2f, 3f, 1.5f, 3.5f, 2.5f, 2f };
                table.SetWidths(columnWidths);
                table.WidthPercentage = 100;
                table.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

                AddHeederTabble(table, fontShabnam, "نام");
                AddHeederTabble(table, fontShabnam, "نام خانوادگی");
                AddHeederTabble(table, fontShabnam, "ایمیل");
                AddHeederTabble(table, fontShabnam, "سن");
                AddHeederTabble(table, fontShabnam, "تلفن");
                AddHeederTabble(table, fontShabnam, "شهر");
                AddHeederTabble(table, fontShabnam, "نقش");
                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    string rolesString = string.Join(", ", roles);
                   
                    AddCellTable(table, fontIranSanse, user.firstName);
                    AddCellTable(table, fontIranSanse ,user.lastName);
                    AddCellTable(table, fontIranSanse ,user.Email);
                    AddCellTable(table, fontIranSanse, user.age.ToString());
                    AddCellTable(table, fontIranSanse, user.PhoneNumber);
                    AddCellTable(table, fontIranSanse, user.city);
                    AddCellTable(table, fontIranSanse, rolesString);
                }

                string logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Pictuere/Screenshot 2023-09-30 111644.png");
                Image logo = Image.GetInstance(logoPath);
                logo.ScaleToFit(150f, 150f);
                logo.Alignment = Image.ALIGN_CENTER;
                logo.SetAbsolutePosition((document.PageSize.Width - logo.ScaledWidth) / 2, document.BottomMargin / 2);
               
                document.Add(logo);
                document.Add(title);
                document.Add(table);

                document.Close();
            }
            return ms.ToArray();
        }
        private void AddHeederTabble(PdfPTable table,Font font,string text)
        {

            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 10,
                BackgroundColor = new BaseColor(25, 25,70),
                RunDirection = PdfWriter.RUN_DIRECTION_RTL
            };
            table.AddCell(cell);
            
        }
        protected void AddCellTable(PdfPTable table,Font font ,string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Padding = 10,
                BackgroundColor =BaseColor.WHITE,
                BorderColor=BaseColor.BLACK,
                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
               
            };
            table.AddCell(cell);
        }
    }
}
