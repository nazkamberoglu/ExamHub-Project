using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.PortableExecutable;

namespace ExamHubProject.Models.Entities
{
    public class StudentExam
    {
        [Key]
        public int StudentExamID { get; set; }
        [ForeignKey("Student")]
        public int RefStudentID { get; set; }


        public virtual Student Student { get; set; }

        [ForeignKey("Exam")]
        public int RefExamID { get; set; }

        public virtual Exam Exam { get; set; }

        public DateTime ExamDate { get; set; }= DateTime.Now;

        public string? StudentAnswersJson { get; set; }

     
        public decimal ExamScore { get; set; }
    }
}
