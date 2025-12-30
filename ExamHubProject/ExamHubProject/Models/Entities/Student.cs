using System.ComponentModel.DataAnnotations;

namespace ExamHubProject.Models.Entities
{
    public class Student
    {
        [Key]
        public int StudentID { get; set; }
        [Required(ErrorMessage ="Name is required")]
        [Display(Name="Student Name")]
        public string StudentName { get; set; }
        [Display(Name ="Student Lastname")]
        [Required(ErrorMessage ="Lastname is required")]
        public string StudentLastName { get; set; }
        [Required(ErrorMessage ="UserName is required")]
        [Display(Name="User Name")]
        public string UserName { get; set; }
        [Display(Name="Password")]
        public string Password { get; set; }

        public string? Role { get; set; }
        public virtual ICollection<StudentExam> StudentExams { get; set; }

 

    


        
    }
}
