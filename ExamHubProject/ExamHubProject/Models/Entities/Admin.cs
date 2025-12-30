using System.ComponentModel.DataAnnotations;

namespace ExamHubProject.Models.Entities
{
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }
        [Display(Name ="Admin Name")]
        public string AdminName { get; set; }

        public string Password { get; set; }

        public string Role {  get; set; }   
    }
}
