using ExamHubProject.Models.Context;
using ExamHubProject.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExamHubProject.Controllers
{
    [AllowAnonymous]
    public class SecurityController : Controller
    {
        private readonly ProjectContext db;

        public SecurityController(ProjectContext db)
        {
                this.db = db;
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.StudentName) ||
        string.IsNullOrWhiteSpace(student.StudentLastName) ||
        string.IsNullOrWhiteSpace(student.UserName) ||
        string.IsNullOrWhiteSpace(student.Password))
            {
                ViewBag.NullError = "Please fill in all required fields.";
                return View(student);
            }


            var existingUser = await db.Students.FirstOrDefaultAsync(u => u.UserName == student.UserName);
            if (existingUser!= null)
            {
                ViewBag.NameError = "This username is taken";
                return View();  
            }

            student.Role = "Student";
            var hasher = new PasswordHasher<Student>();
            student.Password = hasher.HashPassword(student, student.Password);
            db.Students.Add(student);
           
            await db.SaveChangesAsync();

            var cliamList = new List<Claim>
            {   new Claim(ClaimTypes.NameIdentifier,student.StudentID.ToString()),
                new Claim(ClaimTypes.Name,student.StudentName),
                new Claim(ClaimTypes.Surname,student.StudentLastName),
                new Claim("UserName",student.UserName),
                new Claim(ClaimTypes.Role,student.Role)

            };

            var identity=new ClaimsIdentity(cliamList,CookieAuthenticationDefaults.AuthenticationScheme);
            var principal=new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);




            return RedirectToAction("Index","Site");   
        }


        public IActionResult LogIn()
            { return View(); }

        [HttpPost]
        public async Task<IActionResult> LogIn(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.UserName) ||
                string.IsNullOrWhiteSpace(student.Password))
            {
                ViewBag.Error = "UserName and Password are required.";
                return View();
            }

           
            var user = await db.Students
                .FirstOrDefaultAsync(s => s.UserName == student.UserName);

            if (user == null)
            {
                ViewBag.Error = "User or password is incorrect";
                return View();
            }

            
            var hasher = new PasswordHasher<Student>();

            var result = hasher.VerifyHashedPassword(
                user,
                user.Password,      
                student.Password    
            );

           
            if (result != PasswordVerificationResult.Success)
            {
                ViewBag.Error = "User or password is incorrect";
                return View();
            }

           
            var claimList = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.StudentID.ToString()),
        new Claim(ClaimTypes.Name, user.StudentName ?? ""),
        new Claim(ClaimTypes.Surname, user.StudentLastName ?? ""),
        new Claim("UserName", user.UserName ?? "")
    };

            if (!string.IsNullOrWhiteSpace(user.Role))
                claimList.Add(new Claim(ClaimTypes.Role, user.Role));

            var identity = new ClaimsIdentity(
                claimList,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal
            );

            return RedirectToAction("Index", "Site");
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LogIn");
        }

        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(Admin admin)
        {
            var user = db.Admins
       .FirstOrDefault(x => x.AdminName == admin.AdminName
                         && x.Password == admin.Password);

            if (user == null)
            {
                ViewBag.Error = "Admin name or password is incorrect";
                return View();
            }


            admin.Role = "Admin";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,admin.AdminID.ToString()),
                new Claim(ClaimTypes.Name,admin.AdminName),
                new Claim(ClaimTypes.Role,admin.Role)
            };

            var identity=new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
           
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Dashboard", "Admin");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
