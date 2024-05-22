using ChopSuey.Areas.Identity.Data;
using ChopSuey.ViewModels;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChopSuey.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> RegisterConfirm(RegisterLoginViewModel model, [FromServices] UserManager<ApplicationUser> usermaneger, [FromServices] IEmailSender emailSender)
        {
            ApplicationUser user = await usermaneger.FindByEmailAsync(model.username);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = model.username,
                    firstName = model.firstName,
                    lastName = model.lastName,
                    Email = model.username,
                    EmailConfirmed = true,
                    PhoneNumber = model.phone,
                    age = model.age,
                    city = model.city,

                };
                var status = await usermaneger.CreateAsync(user, model.password);
                if (status.Succeeded)
                {
                    if (await usermaneger.IsInRoleAsync(user, "مشتری") == false)
                    {
                        await usermaneger.AddToRoleAsync(user, "مشتری");
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");

                }
            }
            else
            {
                return RedirectToAction("Index");

            }
        }
        public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> LoginConfirm([FromServices] UserManager<ApplicationUser> userManager,
          [FromServices] SignInManager<ApplicationUser> signInManager, RegisterLoginViewModel models)
        {
            ApplicationUser user = await userManager.FindByEmailAsync(models.username);
            if (user != null)
            {
                var status = await signInManager.PasswordSignInAsync(user, models.password, false, true);
                if (status.Succeeded)
                {
                    await userManager.ResetAccessFailedCountAsync(user);
                    if (await userManager.IsInRoleAsync(user, "مشتری") == true && models.roleName == "مشتری")
                    {

                        return RedirectToAction("Index", "Home");
                    }
                    else if (await userManager.IsInRoleAsync(user, "ادمین") == true && models.roleName == "ادمین")
                    {
                        return RedirectToAction("Index", "Admin");
                    }

                    return RedirectToAction("Index", "Home");
                }
                else if (status.IsLockedOut)
                {

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
        public async Task<IActionResult> Logout([FromServices] SignInManager<ApplicationUser> signInManager)
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> UserPanel(string userid, [FromServices] UserManager<ApplicationUser> userManager)
        {
            ApplicationUser User = await userManager.FindByEmailAsync(userid);

            if (User != null)
            {
                UserProfileViewModel viewModel = new UserProfileViewModel
                {
                    id = User.Id,
                    UserName = User.UserName,
                    firstName = User.firstName,
                    lastName = User.lastName,
                    PhoneNumber = User.PhoneNumber,
                    city = User.city,
                    age = User.age,
                };

                return View(viewModel);
            }
            else
            {

                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> UpdateUserProfile(UserProfileViewModel model, [FromServices] UserManager<ApplicationUser> userManager)
        {
            ApplicationUser user = await userManager.FindByIdAsync(model.id);

            if (user != null)
            {
                user.firstName = model.firstName;
                user.lastName = model.lastName;
                user.PhoneNumber = model.PhoneNumber;
                user.city = model.city;
                user.age = model.age;
                user.UserName = model.UserName;
                user.EmailConfirmed = true;

                var status = await userManager.UpdateAsync(user);

                if (status.Succeeded)
                {

                    return Json("بروز رسانی با موفقیت انجام شد.");
                }
                else
                {

                    return Json("خطا در بروزرسانی");
                }
            }
            else
            {
                return Json("کاربر پیدا نشد");
            }

        }
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, [FromServices] UserManager<ApplicationUser> userManager)
        {
            ApplicationUser user = await userManager.GetUserAsync(User);

            if (user != null)
            {

                var Check = await userManager.CheckPasswordAsync(user, currentPassword);

                if (!Check)
                {
                    return Json("رمز عبور جاری نادرست است.");
                }
                var status = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);

                if (status.Succeeded)
                {
                    return Json("تغییر رمز عبور با موفقیت انجام شد.");
                }
                else
                {
                    return Json("خطا در تغییر رمز عبور");
                }
            }
            else
            {
                return Json("کاربر پیدا نشد");
            }

        }
        public async Task<IActionResult> ResetPassword(string email, [FromServices] UserManager<ApplicationUser> userManager)
        {
            var user = await userManager.FindByEmailAsync(email);


            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            ViewData["token"] = token;
            return Json(token);

        }

        public async Task<IActionResult> ResetPasswordConfirm(ResetPasswordViewModel model, [FromServices] UserManager<ApplicationUser> userManager)
        {

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return Json("کاربر پیدا نشد.");
            }

            var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {

                return Json("بازیابی رمز عبور موفقیت آمیز بود.");
            }
            else
            {

                return Json("خطا در بازیابی رمز عبور.");
            }
        }

    }
}
