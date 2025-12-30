using ExamHubProject.Models.Context;
using ExamHubProject.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ExamHubProject.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ExamController : Controller
    {
        private readonly ProjectContext db;
        public ExamController(ProjectContext db)
        {
            this.db = db;
        }
        public IActionResult Index()
        {
            return View(db.Exams.Include(x=>x.Lesson).ToList());
        }

        public IActionResult Create()
        {
            ViewBag.SelectList = new SelectList(db.Lessons,"LessonID", "LessonName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Exam exam,IFormFile pdfFile, string opticalJson,IFormFile Photo)
        {
            if (pdfFile != null && pdfFile.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(pdfFile.FileName);
                var filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    pdfFile.CopyTo(stream);
                }

                exam.PdfPath = "/pdf/" + fileName;
            }

            if(Photo != null &&  Photo.Length > 0)
            {
                var photoFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                if(!Directory.Exists(photoFolder))
                {
                    Directory.CreateDirectory(photoFolder);
                }

                var filePhoto=Guid.NewGuid().ToString()+ Path.GetExtension(Photo.FileName);
                var path=Path.Combine(photoFolder, filePhoto);

                using(var stream=new  FileStream(path, FileMode.Create))
                {
                    Photo.CopyTo(stream);
                }
                exam.Photo="/img/"+ filePhoto;
            }
            exam.OpticalJson = opticalJson;
            db.Exams.Add(exam);
            db.SaveChanges();

            return RedirectToAction("Index");

        }

        public IActionResult Edit(int id)
        {
            var exam = db.Exams.FirstOrDefault(x => x.ExamID == id);
            if (exam == null)
                return NotFound();

            ViewBag.SelectList = new SelectList(
                db.Lessons,
                "LessonID",
                "LessonName",exam.RefLessonID);
              

            return View(exam);
        }

        [HttpPost]
        public IActionResult Edit(
     Exam exam,
     IFormFile pdfFile,
     IFormFile Photo,
     string OpticalJson)
        {
            var existing = db.Exams.FirstOrDefault(x => x.ExamID == exam.ExamID);
            if (existing == null) return NotFound();

           
            existing.ExamName = exam.ExamName;
            existing.RefLessonID = exam.RefLessonID;
            existing.QuestionCount = exam.QuestionCount;

    
            if (pdfFile != null && pdfFile.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/pdf");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(pdfFile.FileName);
                var path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                pdfFile.CopyTo(stream);

                existing.PdfPath = "/pdf/" + fileName;
            }

            // PHOTO (OPTIONAL)
            if (Photo != null && Photo.Length > 0)
            {
                var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                Directory.CreateDirectory(folder);

                var fileName = Guid.NewGuid() + Path.GetExtension(Photo.FileName);
                var path = Path.Combine(folder, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                Photo.CopyTo(stream);

                existing.Photo = "/img/" + fileName;
            }

            // OPTICAL (ONLY IF CHANGED)
            if (!string.IsNullOrEmpty(OpticalJson))
                existing.OpticalJson = OpticalJson;

            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        { 
            var exam= db.Exams.Where(x=>x.ExamID==id).FirstOrDefault();
            if (exam != null)
            {
                db.Exams.Remove(exam);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();

        }


        public IActionResult Details(int id)
        {
            var exam=db.Exams.Include(x=>x.Lesson).Where(x=>x.ExamID == id).FirstOrDefault();   

            return View(exam);  
        }

      
    }
}
