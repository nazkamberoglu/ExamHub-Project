using ExamHubProject.Models.Context;
using ExamHubProject.Models.Entities;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ExamHubProject.Controllers
{
    [Authorize(Roles ="Student")]
    public class SiteController : Controller
    {
        private readonly ProjectContext db;

        public SiteController(ProjectContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            var studentID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            var student=db.Students.Where(x=>x.StudentID == studentID).FirstOrDefault();
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

                LessonAverages = lessonAverages
                    .Select(x =>
                        x.Average == 0
                            ? 0 
                            : (x.Average <= 1
                                ? Math.Round(x.Average * 100, 2)  
                                : Math.Round(x.Average, 2))       
                    )
                    .ToList(),

                Lessons = db.Lessons.Include(x => x.Exams).ToList(),

                Student=student
            };
            ViewBag.User = student.StudentName + " " + student.StudentLastName;
            return View(vm);

        }


        public IActionResult LessonView(int id)
        {
            var lesson = db.Lessons
          .Include(x => x.Exams)
          .FirstOrDefault(x => x.LessonID == id);

            var vm = new ExamVM
            {
                Lesson = lesson,
                Exams = lesson?.Exams.ToList(),
                Lessons = db.Lessons.ToList()
            };

            return View(vm);


        }

        public IActionResult ExamView(int id)
        {

            var exam = db.Exams.Include(x => x.Lesson).Where(x => x.ExamID == id).FirstOrDefault();

            var vm = new ExamVM
            {
                Exams = db.Exams.Include(x => x.Lesson).ToList(),
                Lessons = db.Lessons.ToList(),
                exam = exam



            };

            return View(vm);
        }

 
        [HttpPost]
        public IActionResult SubmitExam([FromBody] ExamSubmitDTO dto)
        {
            var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var exam = db.Exams.FirstOrDefault(x => x.ExamID == dto.ExamId);
            if (exam == null) return NotFound();

            var correctAnswers = JsonConvert.DeserializeObject<List<AnswerDTO>>(exam.OpticalJson);

            int correctCount = dto.Answers.Count(a =>
                correctAnswers.Any(c => c.QuestionNo == a.QuestionNo && c.Answer == a.Answer)
            );

            var studentExam = new StudentExam
            {
                RefStudentID = studentId,
                RefExamID = dto.ExamId,
                StudentAnswersJson = JsonConvert.SerializeObject(dto.Answers),
                ExamDate = DateTime.Now,
                ExamScore = ((decimal)correctCount / correctAnswers.Count) * 100
            };

            db.StudentExams.Add(studentExam);
            db.SaveChanges();

            return Json(new
            {
                score = studentExam.ExamScore,
                correctAnswers = correctAnswers
            });
        }
  
      public IActionResult MyAverages()
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

                LessonAverages = lessonAverages
                    .Select(x =>
                        x.Average == 0
                            ? 0
                            : (x.Average <= 1
                                ? Math.Round(x.Average * 100, 2)
                                : Math.Round(x.Average, 2))
                    )
                    .ToList(),

                Lessons = db.Lessons.Include(x => x.Exams).ToList()
            };

            return View(vm);

        }


        public IActionResult Search(string q)
        {
            var vm = new ExamVM
            {
                Lessons = db.Lessons.Include(x => x.Exams).ToList(),
                SearchExams = new List<Exam>()
            };

            if (!string.IsNullOrWhiteSpace(q))
            {
                vm.SearchExams = db.Exams
                    .Include(x => x.Lesson)
                    .Where(x =>
                        x.ExamName.Contains(q) ||
                        x.Lesson.LessonName.Contains(q))
                    .ToList();
            }

            return View(vm);
        }
    }
}


