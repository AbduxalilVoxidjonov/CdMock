using Microsoft.AspNetCore.Mvc;

namespace CdMock.Controllers.Mock
{
    public class WritingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
