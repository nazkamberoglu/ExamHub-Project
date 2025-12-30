using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExamHubProject.Controllers
{
    public class AdminController : Controller
    {
        [Authorize(Roles="Admin")]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
