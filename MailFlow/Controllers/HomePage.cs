using Microsoft.AspNetCore.Mvc;

namespace MailFlow.Controllers
{
    public class HomePage : Controller
    {
        public IActionResult MainPage()
        {
            return View();
        }
    }
}
