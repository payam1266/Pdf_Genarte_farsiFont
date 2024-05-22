using ChopSuey.Areas.Identity.Data;
using ChopSuey.Contracts;
using ChopSuey.Data;
using ChopSuey.Models;
using ChopSuey.Services;
using ChopSuey.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChopSuey.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DbWebChopSuey _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserPdfGenareator _userPdfGenareator;

        public HomeController(ILogger<HomeController> logger, DbWebChopSuey db, UserManager<ApplicationUser> userManager, UserPdfGenareator userPdfGenareator)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _userPdfGenareator = userPdfGenareator;
        }

        public IActionResult Index()
        {
            _db.Database.EnsureCreated();
            return View();
        }
        public async Task<IActionResult> GenerateUserPdfTable(string userId)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var pdfTable = await _userPdfGenareator.GenerateUserPdfTable(user);

            var document = new Document(PageSize.A4);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            document.Open();
            document.Add(pdfTable);
            document.Close();

            byte[] byteArray = ms.ToArray();
            ms.Close();

            return File(byteArray, "application/pdf", $"{user.firstName}_{user.lastName}.pdf");
        }
    }
}