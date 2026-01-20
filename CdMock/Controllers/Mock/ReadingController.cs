using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CdMock.Data;
using CdMock.Models.Mock;

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

        // GET: Reading/Index?mockId=5 - Barcha reading passages
        public async Task<IActionResult> Index(int mockId)
        {
            var mock = await _context.Mocks.FindAsync(mockId);
            if (mock == null)
            {
                return NotFound();
            }

            ViewBag.MockId = mockId;
            ViewBag.MockTitle = mock.Title;

            var readings = await _context.Readings
                .Include(r => r.Questions)
                .Where(r => r.MockId == mockId)
                .OrderBy(r => r.OrderNumber)
                .ToListAsync();

            return View(readings);
        }

        // GET: Reading/AddPassage?mockId=5 - Passage qo'shish sahifasi
        public async Task<IActionResult> AddPassage(int mockId)
        {
            var mock = await _context.Mocks.FindAsync(mockId);
            if (mock == null)
            {
                return NotFound();
            }

            ViewBag.MockId = mockId;
            ViewBag.MockTitle = mock.Title;

            return View();
        }

        // POST: Reading/AddPassage - Passage saqlash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPassage([Bind("MockId,Title,PassageText,OrderNumber")] Reading reading)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reading);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"✅ {reading.Title} muvaffaqiyatli qo'shildi!";
                return RedirectToAction(nameof(Index), new { mockId = reading.MockId });
            }

            ViewBag.MockId = reading.MockId;
            return View(reading);
        }

        // GET: Reading/EditPassage/5 - Passage tahrirlash
        public async Task<IActionResult> EditPassage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reading = await _context.Readings.FindAsync(id);
            if (reading == null)
            {
                return NotFound();
            }

            ViewBag.MockId = reading.MockId;
            return View(reading);
        }

        // POST: Reading/EditPassage/5 - Passage yangilash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPassage(int id, [Bind("Id,MockId,Title,PassageText,OrderNumber")] Reading reading)
        {
            if (id != reading.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reading);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"✅ {reading.Title} muvaffaqiyatli yangilandi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReadingExists(reading.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { mockId = reading.MockId });
            }

            ViewBag.MockId = reading.MockId;
            return View(reading);
        }

        // POST: Reading/DeletePassage/5 - Passage o'chirish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePassage(int id)
        {
            var reading = await _context.Readings.FindAsync(id);
            if (reading == null)
            {
                TempData["Error"] = "Passage topilmadi!";
                return RedirectToAction(nameof(Index));
            }

            int mockId = reading.MockId;
            _context.Readings.Remove(reading);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"✅ {reading.Title} muvaffaqiyatli o'chirildi!";
            return RedirectToAction(nameof(Index), new { mockId });
        }

        // GET: Reading/AddQuestion?readingId=5 - Savol qo'shish sahifasi
        public async Task<IActionResult> AddQuestion(int readingId)
        {
            var reading = await _context.Readings
                .Include(r => r.Mock)
                .FirstOrDefaultAsync(r => r.Id == readingId);

            if (reading == null)
            {
                return NotFound();
            }

            ViewBag.ReadingId = readingId;
            ViewBag.ReadingTitle = reading.Title;
            ViewBag.MockId = reading.MockId;

            // Keyingi savol raqamini aniqlash
            var lastQuestion = await _context.ReadingQuestions
                .Where(q => q.ReadingId == readingId)
                .OrderByDescending(q => q.OrderNumber)
                .FirstOrDefaultAsync();

            ViewBag.NextOrderNumber = (lastQuestion?.OrderNumber ?? 0) + 1;

            return View();
        }

        // POST: Reading/AddQuestion - Savol saqlash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion([Bind("ReadingId,QuestionText,QuestionType,CorrectAnswer,OptionA,OptionB,OptionC,OptionD,OrderNumber,Points")] ReadingQuestion question)
        {
            if (ModelState.IsValid)
            {
                _context.Add(question);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"✅ Savol {question.OrderNumber} muvaffaqiyatli qo'shildi!";

                // Yana savol qo'shish uchun qaytish
                return RedirectToAction(nameof(AddQuestion), new { readingId = question.ReadingId });
            }

            ViewBag.ReadingId = question.ReadingId;
            return View(question);
        }

        // GET: Reading/EditQuestion/5 - Savol tahrirlash
        public async Task<IActionResult> EditQuestion(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var question = await _context.ReadingQuestions
                .Include(q => q.Reading)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            ViewBag.ReadingId = question.ReadingId;
            ViewBag.ReadingTitle = question.Reading.Title;
            return View(question);
        }

        // POST: Reading/EditQuestion/5 - Savol yangilash
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(int id, [Bind("Id,ReadingId,QuestionText,QuestionType,CorrectAnswer,OptionA,OptionB,OptionC,OptionD,OrderNumber,Points")] ReadingQuestion question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "✅ Savol muvaffaqiyatli yangilandi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                var reading = await _context.Readings.FindAsync(question.ReadingId);
                return RedirectToAction(nameof(ManageQuestions), new { readingId = question.ReadingId });
            }

            ViewBag.ReadingId = question.ReadingId;
            return View(question);
        }

        // POST: Reading/DeleteQuestion/5 - Savol o'chirish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.ReadingQuestions.FindAsync(id);
            if (question == null)
            {
                TempData["Error"] = "Savol topilmadi!";
                return RedirectToAction(nameof(Index));
            }

            int readingId = question.ReadingId;
            _context.ReadingQuestions.Remove(question);
            await _context.SaveChangesAsync();

            TempData["Success"] = "✅ Savol muvaffaqiyatli o'chirildi!";
            return RedirectToAction(nameof(ManageQuestions), new { readingId });
        }

        // GET: Reading/ManageQuestions?readingId=5 - Barcha savollarni boshqarish
        public async Task<IActionResult> ManageQuestions(int readingId)
        {
            var reading = await _context.Readings
                .Include(r => r.Mock)
                .Include(r => r.Questions)
                .FirstOrDefaultAsync(r => r.Id == readingId);

            if (reading == null)
            {
                return NotFound();
            }

            ViewBag.ReadingId = readingId;
            ViewBag.ReadingTitle = reading.Title;
            ViewBag.MockId = reading.MockId;

            var questions = reading.Questions.OrderBy(q => q.OrderNumber).ToList();
            return View(questions);
        }

        // GET: Reading/PreviewPassage/5 - Passage ko'rish (preview)
        public async Task<IActionResult> PreviewPassage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reading = await _context.Readings
                .Include(r => r.Mock)
                .Include(r => r.Questions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reading == null)
            {
                return NotFound();
            }

            return View(reading);
        }

        private bool ReadingExists(int id)
        {
            return _context.Readings.Any(e => e.Id == id);
        }

        private bool QuestionExists(int id)
        {
            return _context.ReadingQuestions.Any(e => e.Id == id);
        }
    }
}