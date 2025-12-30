using ExamHubProject.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace ExamHubProject.Models.Context
{
    public class ProjectContext:DbContext
    {
        public ProjectContext()
        {
            
        }

        public ProjectContext(DbContextOptions<ProjectContext>options):base(options) 
        {
               
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=NAZ01\\SQLExpress;Database=ExamHubDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lesson>()
                .HasData(
                new Lesson { LessonID=1,LessonName="Türkçe" },
                new Lesson { LessonID=2,LessonName="Matematik"},
                new Lesson { LessonID=3,LessonName="Geometri"},
                new Lesson { LessonID=4,LessonName="Biyoloji"},
                new Lesson { LessonID=5,LessonName="Kimya"},
                new Lesson { LessonID=6,LessonName="Fizik"},
                new Lesson { LessonID=7,LessonName="Tarih"},
                new Lesson { LessonID=8,LessonName="Coğrafya" }                       
                
               );

            modelBuilder.Entity<Admin>().HasData
                (new Admin { AdminID = 1, AdminName ="Admin", Password ="123456",Role="Admin" });

        }


        public virtual DbSet<Lesson> Lessons { get; set; }
        public virtual DbSet<Exam> Exams { get; set; }
        public virtual DbSet<Student> Students { get; set; }

        public virtual DbSet<StudentExam> StudentExams { get; set; }

        public virtual DbSet<Admin> Admins { get; set; }
    }
}
