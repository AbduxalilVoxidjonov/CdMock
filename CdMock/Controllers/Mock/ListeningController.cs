using Microsoft.AspNetCore.Mvc;

namespace CdMock.Controllers.Mock
{
    public class ListeningController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
