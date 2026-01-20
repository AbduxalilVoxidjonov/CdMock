using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CdMock.Data;
using CdMock.Models.Mock;

namespace CdMock.Controllers
{
    [Authorize]
    public class MockController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MockController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Mock/Index - Barcha mocklar ro'yxati
        public async Task<IActionResult> Index()
        {
            var mocks = await _context.Mocks
                .Include(m => m.Readings)
                .Include(m => m.Listenings)
                .Include(m => m.Writings)
                .Include(m => m.UserMockResults)
                .OrderByDescending(m => m.CreatedDate)
                .ToListAsync();

            return View(mocks);
        }

        // GET: Mock/Details/5 - Mock tafsilotlari
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mock = await _context.Mocks
                .Include(m => m.Readings)
                    .ThenInclude(r => r.Questions)
                .Include(m => m.Listenings)
                    .ThenInclude(l => l.Questions)
                .Include(m => m.Writings)
                .Include(m => m.UserMockResults)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mock == null)
            {
                return NotFound();
            }

            return View(mock);
        }

        // GET: Mock/Create - Mock yaratish sahifasi (Admin only)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Default qiymatlar bilan yangi Mock obyekti
            var mock = new Mocks
            {
                TimeLimit = 180, // Default 180 daqiqa
                IsActive = true  // Default faol
            };

            return View(mock);
        }

        // POST: Mock/Create - Mock saqlash
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Mocks mock)
        {
            try
            {
                // Model validation
                if (string.IsNullOrWhiteSpace(mock.Title))
                {
                    ModelState.AddModelError("Title", "Test nomi majburiy!");
                }

                if (mock.TimeLimit < 1 || mock.TimeLimit > 300)
                {
                    ModelState.AddModelError("TimeLimit", "Vaqt 1 dan 300 daqiqagacha bo'lishi kerak!");
                }

                if (!ModelState.IsValid)
                {
                    return View(mock);
                }

                // Yaratilgan sanani o'rnatish
                mock.CreatedDate = DateTime.Now;

                // Database'ga qo'shish
                _context.Mocks.Add(mock);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"✅ '{mock.Title}' muvaffaqiyatli yaratildi!";
                return RedirectToAction(nameof(Details), new { id = mock.Id });
            }
            catch (Exception ex)
            {
                // Xatolikni log qilish (production'da logger ishlating)
                TempData["Error"] = $"❌ Xatolik yuz berdi: {ex.Message}";
                return View(mock);
            }
        }

        // GET: Mock/Edit/5 - Mock tahrirlash sahifasi
        [Authorize(Roles = "Admin")]
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

        // POST: Mock/Edit/5 - Mock yangilash
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Mocks mock)
        {
            if (id != mock.Id)
            {
                return NotFound();
            }

            try
            {
                if (string.IsNullOrWhiteSpace(mock.Title))
                {
                    ModelState.AddModelError("Title", "Test nomi majburiy!");
                }

                if (!ModelState.IsValid)
                {
                    return View(mock);
                }

                _context.Update(mock);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"✅ '{mock.Title}' muvaffaqiyatli yangilandi!";
                return RedirectToAction(nameof(Details), new { id = mock.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MockExists(mock.Id))
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
                TempData["Error"] = $"❌ Xatolik: {ex.Message}";
                return View(mock);
            }
        }

        // POST: Mock/Delete/5 - Mock o'chirish
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var mock = await _context.Mocks.FindAsync(id);
                if (mock == null)
                {
                    TempData["Error"] = "Mock topilmadi!";
                    return RedirectToAction(nameof(Index));
                }

                _context.Mocks.Remove(mock);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"✅ '{mock.Title}' muvaffaqiyatli o'chirildi!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ O'chirishda xatolik: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Mock/TakeTest/5 - Test topshirish sahifasi
        public async Task<IActionResult> TakeTest(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mock = await _context.Mocks
                .Include(m => m.Readings)
                    .ThenInclude(r => r.Questions)
                .Include(m => m.Listenings)
                    .ThenInclude(l => l.Questions)
                .Include(m => m.Writings)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (mock == null)
            {
                return NotFound();
            }

            if (!mock.IsActive)
            {
                TempData["Error"] = "Bu test hozirda faol emas!";
                return RedirectToAction(nameof(Index));
            }

            return View(mock);
        }

        // POST: Mock/SubmitTest - Testni yuborish
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTest(int mockId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                var mock = await _context.Mocks
                    .Include(m => m.Readings)
                        .ThenInclude(r => r.Questions)
                    .Include(m => m.Listenings)
                        .ThenInclude(l => l.Questions)
                    .Include(m => m.Writings)
                    .FirstOrDefaultAsync(m => m.Id == mockId);

                if (mock == null)
                {
                    TempData["Error"] = "Test topilmadi!";
                    return RedirectToAction(nameof(Index));
                }

                // Yangi UserMockResult yaratish
                var userMockResult = new UserMockResult
                {
                    MockId = mockId,
                    UserId = userId,
                    StartedAt = DateTime.Now,
                    IsCompleted = false
                };

                _context.UserMockResults.Add(userMockResult);
                await _context.SaveChangesAsync();

                // Reading javoblarini saqlash va tekshirish
                int readingScore = 0;
                if (mock.Readings != null)
                {
                    foreach (var reading in mock.Readings)
                    {
                        if (reading.Questions != null)
                        {
                            foreach (var question in reading.Questions)
                            {
                                var userAnswer = Request.Form[$"reading_{question.Id}"].ToString();

                                if (!string.IsNullOrEmpty(userAnswer))
                                {
                                    bool isCorrect = userAnswer.Trim().Equals(question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);

                                    var readingAnswer = new UserReadingAnswer
                                    {
                                        UserMockResultId = userMockResult.Id,
                                        ReadingQuestionId = question.Id,
                                        UserAnswer = userAnswer,
                                        IsCorrect = isCorrect,
                                        AnsweredAt = DateTime.Now
                                    };

                                    _context.UserReadingAnswers.Add(readingAnswer);

                                    if (isCorrect)
                                    {
                                        readingScore += question.Points;
                                    }
                                }
                            }
                        }
                    }
                }

                // Listening javoblarini saqlash va tekshirish
                int listeningScore = 0;
                if (mock.Listenings != null)
                {
                    foreach (var listening in mock.Listenings)
                    {
                        if (listening.Questions != null)
                        {
                            foreach (var question in listening.Questions)
                            {
                                var userAnswer = Request.Form[$"listening_{question.Id}"].ToString();

                                if (!string.IsNullOrEmpty(userAnswer))
                                {
                                    bool isCorrect = userAnswer.Trim().Equals(question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);

                                    var listeningAnswer = new UserListeningAnswer
                                    {
                                        UserMockResultId = userMockResult.Id,
                                        ListeningQuestionId = question.Id,
                                        UserAnswer = userAnswer,
                                        IsCorrect = isCorrect,
                                        AnsweredAt = DateTime.Now
                                    };

                                    _context.UserListeningAnswers.Add(listeningAnswer);

                                    if (isCorrect)
                                    {
                                        listeningScore += question.Points;
                                    }
                                }
                            }
                        }
                    }
                }

                // Writing javoblarini saqlash (baholanmagan)
                if (mock.Writings != null)
                {
                    foreach (var writing in mock.Writings)
                    {
                        var userAnswer = Request.Form[$"writing_{writing.Id}"].ToString();

                        if (!string.IsNullOrEmpty(userAnswer))
                        {
                            int wordCount = userAnswer.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;

                            var writingAnswer = new UserWritingAnswer
                            {
                                UserMockResultId = userMockResult.Id,
                                WritingId = writing.Id,
                                UserAnswer = userAnswer,
                                WordCount = wordCount,
                                SubmittedAt = DateTime.Now
                            };

                            _context.UserWritingAnswers.Add(writingAnswer);
                        }
                    }
                }

                // Natijani yangilash
                userMockResult.ReadingScore = readingScore;
                userMockResult.ListeningScore = listeningScore;
                userMockResult.WritingScore = 0; // Admin baholaydi
                userMockResult.TotalScore = readingScore + listeningScore;
                userMockResult.IsCompleted = true;
                userMockResult.CompletedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = "✅ Test muvaffaqiyatli yakunlandi!";
                return RedirectToAction(nameof(ViewResult), new { id = userMockResult.Id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"❌ Test yuborishda xatolik: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Mock/ViewResult/5 - Test natijasini ko'rish
        public async Task<IActionResult> ViewResult(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userMockResult = await _context.UserMockResults
                .Include(r => r.Mock)
                .Include(r => r.ReadingAnswers)
                    .ThenInclude(a => a.ReadingQuestion)
                .Include(r => r.ListeningAnswers)
                    .ThenInclude(a => a.ListeningQuestion)
                .Include(r => r.WritingAnswers)
                    .ThenInclude(a => a.Writing)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (userMockResult == null)
            {
                return NotFound();
            }

            // Faqat o'z natijasini yoki admin ko'rishi mumkin
            var userId = _userManager.GetUserId(User);
            if (userMockResult.UserId != userId && !User.IsInRole("Admin"))
            {
                TempData["Error"] = "Siz bu natijani ko'ra olmaysiz!";
                return RedirectToAction(nameof(Index));
            }

            return View(userMockResult);
        }

        // GET: Mock/MyResults - Foydalanuvchining barcha natijalari
        public async Task<IActionResult> MyResults()
        {
            var userId = _userManager.GetUserId(User);

            var results = await _context.UserMockResults
                .Include(r => r.Mock)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CompletedAt)
                .ToListAsync();

            return View(results);
        }

        // GET: Mock/AllResults - Barcha userlarning natijalari (Admin only)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllResults(int? mockId)
        {
            IQueryable<UserMockResult> query = _context.UserMockResults
                .Include(r => r.Mock)
                .Include(r => r.User);

            if (mockId.HasValue)
            {
                query = query.Where(r => r.MockId == mockId);
                ViewBag.MockId = mockId;
            }

            var results = await query
                .OrderByDescending(r => r.TotalScore)
                .ToListAsync();

            return View(results);
        }

        private bool MockExists(int id)
        {
            return _context.Mocks.Any(e => e.Id == id);
        }
    }
}