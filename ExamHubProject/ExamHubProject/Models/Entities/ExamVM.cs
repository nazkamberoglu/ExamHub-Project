namespace ExamHubProject.Models.Entities
{
    public class ExamVM
    {
        public List<Exam> Exams { get; set; }

        public  List<Lesson> Lessons { get; set; }

        public List<OpticalDTO> OpticalDTOs { get; set; }

        public Lesson Lesson { get; set; }

       public Exam exam { get; set; }

        public List<StudentExam> studentExams { get; set; }

        public Student Student { get; set; }

        public List<string> LessonNames { get; set; }
        public List<decimal> LessonAverages { get; set; }

        public List<Exam> SearchExams { get; set; }  
    }
}
