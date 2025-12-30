using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamHubProject.Models.Entities
{
    public class Exam
    {
        [Key]
        public int ExamID { get; set; }
        public string ExamName { get; set; }
       
        
        [ForeignKey("Lesson")]
        public int RefLessonID { get; set; }    

        public virtual Lesson Lesson { get; set; }

        public virtual ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
   
        public int QuestionCount { get; set; }  
        public string? PdfPath { get; set; } 
        public string? OpticalJson { get; set; }

        public string? Photo {  get; set; }


    }
}
