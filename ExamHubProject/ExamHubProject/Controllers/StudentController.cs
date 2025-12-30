using ExamHubProject.Models.Context;
using ExamHubProject.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExamHubProject.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ProjectContext db;

        public StudentController(ProjectContext db)
        {
            this.db = db; 
        }
     

        public IActionResult ExamView(int id)
        {
            var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var exams = db.StudentExams.Include(x => x.Exam).Where(x => x.RefStudentID == studentId && x.Exam.RefLessonID == id).OrderBy(x=>x.ExamDate).ToList();

            if(!exams.Any())
            {
                ViewBag.Error = "There is no past exam data available for this course yet.";
            }

            var vm = new ExamVM
            {
                Lessons = db.Lessons.ToList(),
                studentExams = exams
            };

            return View(vm);    
        }

        public IActionResult MyAccount()
        {
            int userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var student=db.Students.Where(x=>x.StudentID==userID).FirstOrDefault();


            var vm = new ExamVM
            {
                Student = student,
                Lessons=db.Lessons.ToList()
            };
            return View(vm);   


        }


        public IActionResult  Verification()
        {
            var studentID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var student = db.Students.FirstOrDefault(x => x.StudentID == studentID);
            if (student == null) return Unauthorized();

            var vm = new ExamVM
            {
                Student = student,
                Exams = db.Exams.Include(x => x.Lesson).ToList(),
                Lessons = db.Lessons.Include(x => x.Exams).ToList()
            };


            return View(vm);
        }
        [HttpPost]
         public IActionResult Verification(string password)
        {
            var studentID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var student = db.Students.Where(x => x.StudentID == studentID).FirstOrDefault();

            var studentPassword = student.Password;

            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.ErrorNull = "Please enter a valid password ";
                return View();
            }


            if (password == studentPassword)
            {
                return RedirectToAction("Edit", "Student");
            }
            else
            {
                ViewBag.Error = "Password is incorrect";
                return View();
            }
        }


        public IActionResult Edit()
        {
            var userID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = db.Students.FirstOrDefault(x => x.StudentID == userID);

            var vm = new ExamVM
            {
                Student = user,
                Exams = db.Exams.Include(x => x.Lesson).ToList(),
                Lessons = db.Lessons.Include(x => x.Exams).ToList()
            };
            return View(vm);
          
            
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Student student)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (idClaim == null)
                return RedirectToAction("LogIn", "Security");

            int userID = int.Parse(idClaim.Value);

            var user = db.Students.FirstOrDefault(x => x.StudentID == userID);
            if (user == null) return NotFound();

   
            if (string.IsNullOrWhiteSpace(student.StudentName) ||
                string.IsNullOrWhiteSpace(student.StudentLastName) ||
                string.IsNullOrWhiteSpace(student.UserName))
            {
                ViewBag.Error = "Please fill in all required fields.";

                var errorVm = new ExamVM
                {
                    Student = user,
                    Exams = db.Exams.Include(x => x.Lesson).ToList(),
                    Lessons = db.Lessons.Include(x => x.Exams).ToList()
                };

                return View(errorVm);
            }

            user.StudentName = student.StudentName;
            user.StudentLastName = student.StudentLastName;
            user.UserName = student.UserName;

      
            user.Password = student.Password;

            db.SaveChanges();

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.StudentID.ToString()),
        new Claim(ClaimTypes.Name, user.StudentName ?? ""),
        new Claim(ClaimTypes.Surname, user.StudentLastName ?? ""),
        new Claim("UserName", user.UserName ?? ""),
        new Claim(ClaimTypes.Role, user.Role ?? "Student")
    };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)
                )
            );

            var vm = new ExamVM
            {
                Student = user,
                Exams = db.Exams.Include(x => x.Lesson).ToList(),
                Lessons = db.Lessons.Include(x => x.Exams).ToList()
            };

            ViewBag.Info = "Your information has been successfully updated.";
            return View(vm);
        }


        public IActionResult AverageScoreChart()
        {
            var studentID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

          
            var data = db.StudentExams
                .Include(x => x.Exam)
                .ThenInclude(e => e.Lesson)
                .Where(x => x.RefStudentID == studentID)
                .ToList();

            var lessonAverages = data
                .GroupBy(x => x.Exam.Lesson.LessonName)
                .Select(g => new
                {
                    LessonName = g.Key,
                    Average = g.Average(x => x.ExamScore)
                })
                .ToList();

            var vm = new ExamVM
            {
                LessonNames = lessonAverages.Select(x => x.LessonName).ToList(),
                LessonAverages = lessonAverages.Select(x => x.Average).ToList()
            };

            return View(vm);
        }
    }
}
