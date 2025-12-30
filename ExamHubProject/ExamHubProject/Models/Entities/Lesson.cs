using System.ComponentModel.DataAnnotations;

namespace ExamHubProject.Models.Entities
{
    public class Lesson
    {
        [Key]
        public int  LessonID { get; set; }

        public string LessonName { get; set; }

        public virtual ICollection<Exam> Exams { get; set; }   
        
        
    }
}
