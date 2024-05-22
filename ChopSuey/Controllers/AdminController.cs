using ChopSuey.Areas.Identity.Data;
using ChopSuey.Contracts;
using ChopSuey.Data;
using ChopSuey.Services;
using ChopSuey.ViewModels;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChopSuey.Controllers
{
    [Authorize(Policy = ("Adminpolicy"))]
    public class AdminController : Controller
    {
        private readonly DbWebChopSuey _db;
        private readonly IPdfService _pdfService;
        private readonly AllUserPdfGenerator _allUserPdfGenerator;
        private readonly UserPdfGenareator _userPdfGenareator;

        public AdminController(DbWebChopSuey db, IPdfService pdfService,AllUserPdfGenerator allUserPdfGenerator,UserPdfGenareator userPdfGenareator)
        {
            _db = db;
            _pdfService = pdfService;
           _allUserPdfGenerator = allUserPdfGenerator;
           _userPdfGenareator = userPdfGenareator;
        }



        public async Task<IActionResult> GenerateAllUsersPdf()
        {
            
            var pdfTable = await _allUserPdfGenerator.GenerateUserPdfTable();
          
            var document = new Document(PageSize.A4);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            document.Open();
            document.Add(pdfTable);
            document.Close();
      
            byte[] byteArray = ms.ToArray();
            ms.Close();

            return File(byteArray, "application/pdf", "Users.pdf");
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


        public IActionResult Index()
        {
            List<IdentityRole> Roles = _db.Roles.ToList();
            ViewData["Roles"] = Roles;
            return View();
        }
        public IActionResult ListOfUsers()
        {
            var users = _db.Users.ToList();
            List<UserViewModel> lstusers = new List<UserViewModel>();
            users.ForEach(x =>
            {
                var userroles = _db.UserRoles.ToList().Where(y => y.UserId == x.Id).ToList();

                userroles.ForEach(k =>
                {
                    var roles = _db.Roles.ToList().Where(z => z.Id == k.RoleId).ToList();
                    roles.ForEach(h =>
                    {
                        UserViewModel userview = new UserViewModel();

                        userview.rolename = h.Name;
                        userview.name = x.firstName;
                        userview.family = x.lastName;
                        userview.roleId = k.RoleId;
                        userview.id = x.Id;
                        userview.email = x.Email;
                        userview.phone = x.PhoneNumber;
                        userview.age = x.age;
                        userview.city = x.city;
                        lstusers.Add(userview);
                    });
                });
            });
            return Json(lstusers);
        }
        public async Task<IActionResult> EditUsers(string id, [FromServices] UserManager<ApplicationUser> userManager)
        {

            ApplicationUser user = await userManager.FindByIdAsync(id);
            List<UserViewModel> lstuser = new List<UserViewModel>();
            if (user != null)
            {
                var rolId = _db.UserRoles.Where(x => x.UserId == id).ToList();
                rolId.ForEach(z =>
                {
                    var roles = _db.Roles.Where(s => s.Id == z.RoleId).ToList();
                    roles.ForEach(d =>
                    {
                        UserViewModel model = new UserViewModel();
                        model.id = user.Id;
                        model.name = user.firstName;
                        model.family = user.lastName;
                        model.phone = user.PhoneNumber;
                        model.age = user.age;
                        model.city = user.city;
                        model.email = user.Email;
                        model.rolename = d.Name;
                        model.roleId = d.Id;
                        lstuser.Add(model);
                    });
                });
            }
            return Json(lstuser);
        }
        public async Task<IActionResult> UpdateUser(UserViewModel model, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser user = await userManager.FindByIdAsync(model.id);

            if (user == null)
            {
                return Json("user not find");
            }
            user.firstName = model.name;
            user.lastName = model.family;
            user.Email = model.email;
            user.age = model.age;
            user.PhoneNumber = model.phone;
            user.city = model.city;

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return Json("update user not successful");
            }

            var role = await roleManager.FindByIdAsync(model.roleId);

            if (role == null)
            {
                return Json("role not find");
            }

            var currentRoles = await userManager.GetRolesAsync(user);

            var newRole = await roleManager.FindByIdAsync(model.roleId);

            if (newRole == null)
            {
                return Json("role not find");
            }

            await userManager.RemoveFromRolesAsync(user, currentRoles);
            await userManager.AddToRoleAsync(user, newRole.Name);

            return Json("Update user successful");
        }
        public async Task<IActionResult> AddRoleToUser(UserViewModel model, [FromServices] UserManager<ApplicationUser> userManager, [FromServices] RoleManager<IdentityRole> roleManager)
        {

            ApplicationUser user = await userManager.FindByIdAsync(model.id);

            if (user == null)
            {
                return Json("user not find");
            }
            var role = await roleManager.FindByIdAsync(model.roleId);

            if (role == null)
            {
                return Json("role not find");
            }

            var addToRole = await userManager.AddToRoleAsync(user, role.Name);

            if (addToRole.Succeeded)
            {
                return Json("new role add successful");
            }
            else
            {
                return Json("error in adding user role");
            }
        }

        public async Task<IActionResult> DeleteUser(string id, [FromServices] UserManager<ApplicationUser> usermanager)
        {
            ApplicationUser user = await usermanager.FindByIdAsync(id);
            if (user != null)
            {
                await usermanager.DeleteAsync(user);
                return Json("delete confirm");
            }
            else
            {
                return Json("id not find");
            }
        }
        public async Task<IActionResult> ListOfCustomers([FromServices] UserManager<ApplicationUser> userManager)
        {
            var customers = await userManager.GetUsersInRoleAsync("مشتری");
            var lstCustomer = customers.Select(user => new
            {
                id = user.Id,
                name = user.firstName,
                family = user.lastName,
                email = user.Email,
                phone = user.PhoneNumber,
                age = user.age,
                city = user.city,
                rolename = "مشتری"
            }).ToList();

            return Json(lstCustomer);
        }
        public async Task<IActionResult> ListOfAdmins([FromServices] UserManager<ApplicationUser> userManager)
        {
            var admins = await userManager.GetUsersInRoleAsync("ادمین");
            var lstadmin = admins.Select(user => new
            {
                id = user.Id,
                name = user.firstName,
                family = user.lastName,
                email = user.Email,
                phone = user.PhoneNumber,
                age = user.age,
                city = user.city,
                rolename = "ادمین"
            }).ToList();

            return Json(lstadmin);
        }
        public async Task<IActionResult> GeneratePdf_UserInfo(string userId)
        {
            var pdfBytes = await _pdfService.CreatePdfAsync(userId);

            return File(pdfBytes, "application/pdf", "UserInformation.pdf");
        }
        public async Task<IActionResult> GeneratePdf_AllUsers()
        {
            var pdfBytes = await _pdfService.GenerateUserListPdfAsync();

            return File(pdfBytes, "application/pdf", "UserInformation.pdf");
        }
    }
}
