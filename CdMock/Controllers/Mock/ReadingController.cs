using CdMock.Data;
using CdMock.Models.Reading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CdMock.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReadingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReadingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ReadingTexts
        public async Task<IActionResult> Index()
        {
            var readingTexts = await _context.ReadingTexts
                .Include(r => r.Mocks)
                .OrderByDescending(r => r.TextId)
                .ToListAsync();

            // Agar malumot bo'lmasa
            if (!readingTexts.Any())
            {
                ViewBag.Message = "Hozircha reading text yo'q";
            }

            return View(readingTexts);
        }

        // GET: ReadingTexts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readingText = await _context.ReadingTexts
                .Include(r => r.Mocks)
                .FirstOrDefaultAsync(m => m.TextId == id);

            if (readingText == null)
            {
                return NotFound();
            }

            return View(readingText);
        }

        // GET: ReadingTexts/Create
        public IActionResult Create()
        {
            // Mocklar ro'yxatini olish
            var activeMocks = _context.Mocks
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new { m.Id, m.Name })
                .ToList();

            // Debugging uchun
            Console.WriteLine($"Aktiv mocklar soni: {activeMocks.Count}");

            // Agar mocklar bo'lmasa
            if (!activeMocks.Any())
            {
                TempData["Error"] = "Avval Mock yaratish kerak! Aktiv mock topilmadi.";
                return RedirectToAction("Create", "Mock");
            }

            // ViewBag ga yuklash
            ViewBag.Mocks = new SelectList(activeMocks, "Id", "Name");
            ViewBag.MocksCount = activeMocks.Count; // Debugging uchun

            return View();
        }

        // POST: ReadingTexts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReadingText readingText)
        {
            // DEBUGGING: Qabul qilingan ma'lumotlarni tekshirish
            Console.WriteLine($"=== CREATE POST ===");
            Console.WriteLine($"Title: {readingText.Title}");
            Console.WriteLine($"MockId: {readingText.MockId}");
            Console.WriteLine($"Text length: {readingText.Text?.Length ?? 0}");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            // Xatolarni console ga chiqarish
            if (!ModelState.IsValid)
            {
                Console.WriteLine("=== VALIDATION ERRORS ===");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    if (errors.Any())
                    {
                        Console.WriteLine($"Field: {key}");
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"  Error: {error.ErrorMessage}");
                            if (error.Exception != null)
                            {
                                Console.WriteLine($"  Exception: {error.Exception.Message}");
                            }
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(readingText);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("✅ Muvaffaqiyatli saqlandi!");
                    TempData["Success"] = "Reading text muvaffaqiyatli qo'shildi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Saqlashda xatolik: {ex.Message}");
                    ModelState.AddModelError("", $"Saqlashda xatolik: {ex.Message}");
                }
            }

            // Agar validation xato bo'lsa, mocklar ro'yxatini qaytadan yuklash
            var activeMocks = _context.Mocks
                .Where(m => m.IsActive)
                .Select(m => new { m.Id, m.Name })
                .ToList();

            ViewBag.Mocks = new SelectList(activeMocks, "Id", "Name", readingText.MockId);

            return View(readingText);
        }

        // GET: ReadingTexts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readingText = await _context.ReadingTexts.FindAsync(id);
            if (readingText == null)
            {
                return NotFound();
            }

            var activeMocks = _context.Mocks
                .Where(m => m.IsActive)
                .Select(m => new { m.Id, m.Name })
                .ToList();

            ViewBag.Mocks = new SelectList(activeMocks, "Id", "Name", readingText.MockId);
            return View(readingText);
        }

        // POST: ReadingTexts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReadingText readingText)
        {
            if (id != readingText.TextId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(readingText);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Reading text muvaffaqiyatli yangilandi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReadingTextExists(readingText.TextId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var activeMocks = _context.Mocks
                .Where(m => m.IsActive)
                .Select(m => new { m.Id, m.Name })
                .ToList();

            ViewBag.Mocks = new SelectList(activeMocks, "Id", "Name", readingText.MockId);
            return View(readingText);
        }

        // GET: ReadingTexts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var readingText = await _context.ReadingTexts
                .Include(r => r.Mocks)
                .FirstOrDefaultAsync(m => m.TextId == id);

            if (readingText == null)
            {
                return NotFound();
            }

            return View(readingText);
        }

        // POST: ReadingTexts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var readingText = await _context.ReadingTexts.FindAsync(id);
            if (readingText != null)
            {
                _context.ReadingTexts.Remove(readingText);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reading text o'chirildi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReadingTextExists(int id)
        {
            return _context.ReadingTexts.Any(e => e.TextId == id);
        }
    }
}