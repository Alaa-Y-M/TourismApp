using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using GradProj.Models;
using GradProj.Services;
using GradProj.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradProj.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountController : Controller
    {
        private readonly ApplicationContext db;
        private readonly UserManager<ApplcationUser> manager;
        private readonly SignInManager<ApplcationUser> signIn;
        private readonly RoleManager<ApplicationRole> roleManager;
        public static string succmsg { set; get; }
        public static string message { set; get; }
        public static string valmssg { set; get; }
        public static string adminmsg { set; get; }
        public static string LoggerName { set; get; }
        public IWebHostEnvironment Hosting { get; }

        public AccountController(ApplicationContext _db, UserManager<ApplcationUser> _manager, SignInManager<ApplcationUser> _signIn,
           RoleManager<ApplicationRole> roleManager,IWebHostEnvironment hosting
           )
        {
            db = _db;
            manager = _manager;
            signIn = _signIn;
            this.roleManager = roleManager;
            Hosting = hosting;
        }

        #region register
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Register")]
        [Obsolete]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var img = await FilePath(model.File);
                var user = new ApplcationUser { Email = model.Email, UserName = model.UserName, PhoneNumber = model.Phone ,ImgUrl=img};
                if (user == null)
                {
                    return NotFound();
                }
                if (!IsEmailValid(model.Email))
                {
                    ViewBag.emailinValid = "Invalid Email!!!!";
                    return View();
                }
                if (EmailExists(model.Email))
                {
                    ViewBag.emailExist = "Email Exists!!";
                    return View();
                }
                if (UserNameExists(model.UserName))
                {
                    ViewBag.userExists = "User Name Exists!!";
                    return View();
                }
                var result = await manager.CreateAsync(user,model.Password);
                if (result.Succeeded)
                {
                    LoggerName = model.UserName;
                    //https://localhost:44375/RegisterationConfirm?ID=0i080uy7vft6op98t&Token=6rtyt780ou9y765e46900ff
                    var token = await manager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmlink = Url.Action("RegisterationConfirm", "Account", new { ID = user.Id, Token = HttpUtility.UrlEncode(token) }, Request.Scheme);
                    var txt = "Please Confirm UR Registeration To Get More Better Services.";
                    var link = "<a href=\"" + confirmlink + "\">Registeration Confirm</a>";
                    var title = "Registeration";
                    if (await SendGridApi.Execute(Request.Host.ToString(), user.Email, user.UserName, txt, link, title))
                    {
                        ViewBag.success = "Your Registeration Have Done Successfully But U Should Go To Ur Email To Confirm It.";
                        succmsg = "Your Registeration Have Done Successfully But U Should Go To Ur Email To Confirm It.";
                        return View();
                    }
                }
            }
            return View();
        }
        //Email Checking
        private bool IsEmailValid(string email)
        {
            Regex em = new Regex(@"\w+\@\w+\.com|\w+\@\w+\.net");
            return em.IsMatch(email);
        }

        private bool UserNameExists(string userName)
        {
            return db.Users.Any(u => u.UserName == userName);
        }

        private bool EmailExists(string email)
        {
            return db.Users.Any(u => u.Email == email);
        }
        #endregion
        #region Confirmation
        [HttpGet]
        [AllowAnonymous]
        [Route("RegisterationConfirm")]
        public async Task<IActionResult> RegisterationConfirm(string ID, string Token)
        {
            if (string.IsNullOrEmpty(ID) || string.IsNullOrEmpty(Token))
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var user = await manager.FindByIdAsync(ID);
            if (user == null)
                return StatusCode(StatusCodes.Status404NotFound);

            var result = await manager.ConfirmEmailAsync(user, HttpUtility.UrlDecode(Token));

            if (result.Succeeded)
            {
                await manager.AddToRoleAsync(user, "User");
                if (await manager.IsInRoleAsync(user,"User"))
                {
                    AddCookies(user.UserName, user.Id, "User",false);
                }
                valmssg = "Your Account Validated Successfully";
                message = "Loged in";// for drop down
                return RedirectToAction("index", "home");
            }
            return StatusCode(StatusCodes.Status400BadRequest);
        }
        #endregion
        #region Login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            await CreateRole();
            await CreateAdmin();
            if (model == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            var user = await manager.FindByEmailAsync(model.Email);
            if (user == null)
                return StatusCode(StatusCodes.Status404NotFound);
            if (!user.EmailConfirmed)
            {
                ViewBag.email = "Email Has Not Confirmed Yet!!";
                return View();
            }

            var result = await signIn.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                if (await roleManager.RoleExistsAsync("User"))
                {
                    if (!await manager.IsInRoleAsync(user, "User") && !await manager.IsInRoleAsync(user, "Admin"))
                    {
                        await manager.AddToRoleAsync(user, "User");
                    }
                }
                var roleName = await GetRoleNameByUserId(user.Id);
                if (roleName != null)
                {
                    AddCookies(user.UserName, user.Id, roleName, model.RememberMe);
                }
                LoggerName = user.UserName;
                if (roleName == "Admin")
                {
                    adminmsg = "Admin here";
                }
                return RedirectToAction("index", "home");
            }
            else if (result.IsLockedOut)
            {
                return View("Error");
            }
            return StatusCode(StatusCodes.Status204NoContent);
        }

        private async Task<string> GetRoleNameByUserId(string userid)
        {
            var userRole = await db.UserRoles.FirstOrDefaultAsync(x => x.UserId == userid);
            if (userRole != null)
            {
                var roleName = await db.Roles.Where(a => a.Id == userRole.RoleId).Select(a => a.Name).FirstOrDefaultAsync();
                if (roleName != null)
                    return roleName;
            }
            return null;
        }
        #endregion
        private async Task CreateAdmin()
        {
            var user = await manager.FindByNameAsync("Admin");
            if (user == null)
            {
                var admin = new ApplcationUser
                {
                    Email = "Admin@admin.com",
                    UserName = "Admin",
                    EmailConfirmed = true
                };
                var result = await manager.CreateAsync(admin, "Admin_1");
                if (result.Succeeded)
                {
                    if (await roleManager.RoleExistsAsync("Admin"))
                        await manager.AddToRoleAsync(admin, "Admin");

                }
            }
        }
        private async Task CreateRole()
        {
            var role = new ApplicationRole
            {
                Name = "Admin"
            };
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(role);
            }
            role = new ApplicationRole
            {
                Name = "User"
            };
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(role);
            }
        }
        private async void AddCookies(string username, string userid, string role, bool remember)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,username),
                new Claim(ClaimTypes.NameIdentifier,userid),
                new Claim(ClaimTypes.Role,role)

            };
            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            if (remember)
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = remember,
                    IssuedUtc = DateTime.UtcNow.AddDays(10)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), authProperties);
                message = "loggd";
            }
            else
            {
                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = remember,
                    IssuedUtc = DateTime.UtcNow.AddMinutes(30)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimIdentity), authProperties);
                message = "loggd";
            }

        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            message = null;
            adminmsg = null;
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        // GET: /Account/ForgetPassword
        [AllowAnonymous]
        public ActionResult ForgetPassword()
        {
            return View();
        }
        // Post: /Account/ForgetPassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }
            var user = await manager.FindByEmailAsync(email);
            var token = await manager.GeneratePasswordResetTokenAsync(user);
            var newToken = HttpUtility.UrlEncode(token);
            var confirmlink = Url.Action("ResetPassword", "Account", new { ID = user.Id, Token = newToken }, Request.Scheme);
            var txt = "Please Confirm UR Password.";
            var link = "<a href=\"" + confirmlink + "\">Confirm Password</a>";
            var title = "Reset Ur Password";
            if (await SendGridApi.Execute(Request.Host.ToString(), user.Email, user.UserName, txt, link, title))
            {
                return View("login");
            }
            return new ObjectResult(new {Token= newToken });
        }
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        [Route("ResetPassword")]
        public ActionResult ResetPassword(string ID,string Token)
        {
            if (!string.IsNullOrEmpty(ID) || !string.IsNullOrEmpty(Token))
            {
                ViewBag.ID = ID;
                ViewBag.Token = Token;
            }
            return View();
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await manager.FindByIdAsync(model.ID);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("login", "Account");
            }
            var result = await manager.ResetPasswordAsync(user,HttpUtility.UrlDecode(model.Token), model.Password);
            if (result.Succeeded)
            {
                return View("login");
            }
            return View();
        }
        bool isImgValid(string filename)
        {
            var extinsion = Path.GetExtension(filename);
            if (extinsion.Contains(".jpg")) return true;
            if (extinsion.Contains(".jpeg")) return true;
            if (extinsion.Contains(".png")) return true;
            if (extinsion.Contains(".gif")) return true;
            if (extinsion.Contains(".bmb")) return true;
            return false;

        }
        [Obsolete]
        async Task<string> FilePath(IFormFile file)
        {
            string NewFileName = string.Empty;
            if (file != null && file.Length > 0)
            {
                if (isImgValid(file.FileName))
                {
                    string extinsion = Path.GetExtension(file.FileName);
                    NewFileName = Guid.NewGuid().ToString() + extinsion;
                    string FullPath = Path.Combine(Hosting.WebRootPath + "/uploads/users", NewFileName);
                    await file.CopyToAsync(new FileStream(FullPath, FileMode.Create));
                }
                return NewFileName;
            }
            else return NewFileName;

        }
    }
}