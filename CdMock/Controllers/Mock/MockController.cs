using CdMock.Data;
using CdMock.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CdMock.Controllers
{
    [Authorize]
    public class MockController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MockController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Barcha ma'lumotlarni ko'rsatish
        public async Task<IActionResult> Index()
        {
            try
            {
                var mocksFromDb = await _context.Mocks
                    .Include(m => m.ReadingTexts) // ReadingTexts ham yuklanadi
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();

                return View(mocksFromDb);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Xatolik: {ex.Message}";
                return View(new List<Mocks>()); // MockViewModel emas, Mocks!
            }
        }

        // Test uchun Mock va uning ReadingTextlarini yuklash
        [HttpGet]
        public async Task<IActionResult> Test(int id)
        {
            var mock = await _context.Mocks
                .Include(m => m.ReadingTexts) // ReadingTextlarni yuklash
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mock == null)
            {
                return NotFound();
            }

            // Agar mock aktiv bo'lmasa
            if (!mock.IsActive)
            {
                TempData["Error"] = "Bu mock test hozirda mavjud emas!";
                return RedirectToAction("Index");
            }

            return View(mock);
        }

        [HttpPost]
        public async Task<IActionResult> Test(int id, string inputData)
        {
            var mock = await _context.Mocks
                .Include(m => m.ReadingTexts)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mock == null)
            {
                return NotFound();
            }

            ViewBag.ReceivedData = inputData;
            return View(mock);
        }

        // GET: Yangi ma'lumot yaratish sahifasi
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Yangi ma'lumotni saqlash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mocks model) // MockViewModel emas!
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.CreatedAt = DateTime.Now;

                    await _context.Mocks.AddAsync(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Ma'lumot muvaffaqiyatli saqlandi!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Saqlashda xatolik: {ex.Message}");
            }

            return View(model);
        }

        // GET: Tafsilotlarni ko'rish
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mock = await _context.Mocks
                .Include(m => m.ReadingTexts) // ReadingTexts ham ko'rsatiladi
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mock == null)
            {
                return NotFound();
            }

            return View(mock);
        }

        // GET: Tahrirlash sahifasi
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mock = await _context.Mocks.FindAsync(id);

            if (mock == null)
            {
                return NotFound();
            }

            return View(mock);
        }

        // POST: O'zgarishlarni saqlash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mocks model) // MockViewModel emas!
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Ma'lumot muvaffaqiyatli yangilandi!";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MockExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Yangilashda xatolik: {ex.Message}");
                }
            }

            return View(model);
        }

        // GET: O'chirish tasdiqlash sahifasi
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mock = await _context.Mocks
                .Include(m => m.ReadingTexts) // ReadingTexts soni ko'rsatish uchun
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mock == null)
            {
                return NotFound();
            }

            return View(mock);
        }

        // POST: O'chirishni tasdiqlash
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var mock = await _context.Mocks.FindAsync(id);

                if (mock != null)
                {
                    _context.Mocks.Remove(mock);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Ma'lumot muvaffaqiyatli o'chirildi!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"O'chirishda xatolik: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        private bool MockExists(int id)
        {
            return _context.Mocks.Any(e => e.Id == id);
        }
    }
}