using ExamHubProject.Models.Context;
using ExamHubProject.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ExamHubProject.Controllers
{
    [Authorize(Roles="Admin")]
    public class LessonController : Controller
    {

        private readonly ProjectContext db;

        public LessonController(ProjectContext db)
        {
                this.db= db;
        }
        public IActionResult Index()
        {
            return View(db.Lessons.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(Lesson lesson)
        {  
            db.Lessons.Add(lesson);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


       public IActionResult Delete(int id)
        {
            var lesson=db.Lessons.Where(x=>x.LessonID==id).FirstOrDefault();

            if (lesson == null)
            {
                return NotFound();  
            }    
                
            db.Lessons.Remove(lesson);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var lesson=db.Lessons.Where(x=>x.LessonID== id).FirstOrDefault();

            if (lesson==null)
            {
                return NotFound();
            }

            return View(lesson);
         
        }

        [HttpPost]
        public IActionResult Edit(Lesson lesson)
        {
            db.Entry(lesson).State=EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}
