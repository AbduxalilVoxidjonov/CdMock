using CdMock.Data;
using CdMock.Models.Listening;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CdMock.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ListeningController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ListeningController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Listening
        public async Task<IActionResult> Index()
        {
            var listeningAudios = await _context.ListeningAudios
                .Include(l => l.Mocks)
                .OrderByDescending(l => l.UploadedAt)
                .ToListAsync();

            return View(listeningAudios);
        }

        // GET: Listening/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listeningAudio = await _context.ListeningAudios
                .Include(l => l.Mocks)
                .FirstOrDefaultAsync(m => m.AudioId == id);

            if (listeningAudio == null)
            {
                return NotFound();
            }

            return View(listeningAudio);
        }

        // GET: Listening/Create
        public IActionResult Create()
        {
            var activeMocks = _context.Mocks
                .Where(m => m.IsActive)
                .Select(m => new { m.Id, m.Name })
                .ToList();

            if (!activeMocks.Any())
            {
                TempData["Error"] = "Avval Mock yaratish kerak!";
                return RedirectToAction("Create", "Mock");
            }

            ViewBag.Mocks = new SelectList(activeMocks, "Id", "Name");
            return View();
        }

        // POST: Listening/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ListeningAudio model, IFormFile audioFile)
        {
            Console.WriteLine($"=== CREATE LISTENING ===");
            Console.WriteLine($"Title: {model.Title}");
            Console.WriteLine($"MockId: {model.MockId}");
            Console.WriteLine($"AudioFile: {audioFile?.FileName ?? "NULL"}");

            if (audioFile == null || audioFile.Length == 0)
            {
                ModelState.AddModelError("audioFile", "Audio fayl yuklash majburiy!");
            }

            // Fayl formatini tekshirish
            var allowedExtensions = new[] { ".mp3", ".wav", ".m4a", ".ogg" };
            var fileExtension = Path.GetExtension(audioFile?.FileName)?.ToLower();

            if (audioFile != null && !allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("audioFile", "Faqat MP3, WAV, M4A, OGG formatdagi fayllar qabul qilinadi!");
            }

            // Navigation property ni ModelState dan olib tashlash
            ModelState.Remove("Mocks");
            ModelState.Remove("AudioFilePath");
            ModelState.Remove("FileName");

            if (ModelState.IsValid && audioFile != null)
            {
                try
                {
                    // Uploads papkasini yaratish
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "listening");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Noyob fayl nomi yaratish
                    var uniqueFileName = $"{Guid.NewGuid()}_{audioFile.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Faylni saqlash
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await audioFile.CopyToAsync(fileStream);
                    }

                    // Model ma'lumotlarini to'ldirish
                    model.AudioFilePath = $"/uploads/listening/{uniqueFileName}";
                    model.FileName = audioFile.FileName;
                    model.FileSize = audioFile.Length;
                    model.UploadedAt = DateTime.Now;

                    _context.Add(model);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Listening audio muvaffaqiyatli yuklandi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("", $"Yuklashda xatolik: {ex.Message}");
                }
            }

            // Xatolarni ko'rsatish
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"{key}: {error.ErrorMessage}");
                }
            }

            var activeMocks = _context.Mocks
                .Where(m => m.IsActive)
                .Select(m => new { m.Id, m.Name })
                .ToList();

            ViewBag.Mocks = new SelectList(activeMocks, "Id", "Name", model.MockId);
            return View(model);
        }

        // GET: Listening/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listeningAudio = await _context.ListeningAudios.FindAsync(id);
            if (listeningAudio == null)
            {
                return NotFound();
            }

            var activeMocks = _context.Mocks
                .Where(m => m.IsActive)
                .Select(m => new { m.Id, m.Name })
                .ToList();

            ViewBag.Mocks = new SelectList(activeMocks, "Id", "Name", listeningAudio.MockId);
            return View(listeningAudio);
        }

        // POST: Listening/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ListeningAudio model, IFormFile? audioFile)
        {
            if (id != model.AudioId)
            {
                return NotFound();
            }

            ModelState.Remove("Mocks");
            if (audioFile == null)
            {
                ModelState.Remove("AudioFilePath");
                ModelState.Remove("FileName");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Agar yangi fayl yuklangan bo'lsa
                    if (audioFile != null && audioFile.Length > 0)
                    {
                        // Eski faylni o'chirish
                        var oldFilePath = Path.Combine(_environment.WebRootPath, model.AudioFilePath.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }

                        // Yangi faylni yuklash
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "listening");
                        var uniqueFileName = $"{Guid.NewGuid()}_{audioFile.FileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await audioFile.CopyToAsync(fileStream);
                        }

                        model.AudioFilePath = $"/uploads/listening/{uniqueFileName}";
                        model.FileName = audioFile.FileName;
                        model.FileSize = audioFile.Length;
                    }

                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Listening audio muvaffaqiyatli yangilandi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListeningAudioExists(model.AudioId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            var activeMocks = _context.Mocks
                .Where(m => m.IsActive)
                .Select(m => new { m.Id, m.Name })
                .ToList();

            ViewBag.Mocks = new SelectList(activeMocks, "Id", "Name", model.MockId);
            return View(model);
        }

        // GET: Listening/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listeningAudio = await _context.ListeningAudios
                .Include(l => l.Mocks)
                .FirstOrDefaultAsync(m => m.AudioId == id);

            if (listeningAudio == null)
            {
                return NotFound();
            }

            return View(listeningAudio);
        }

        // POST: Listening/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var listeningAudio = await _context.ListeningAudios.FindAsync(id);
            if (listeningAudio != null)
            {
                // Faylni o'chirish
                var filePath = Path.Combine(_environment.WebRootPath, listeningAudio.AudioFilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.ListeningAudios.Remove(listeningAudio);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Listening audio o'chirildi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ListeningAudioExists(int id)
        {
            return _context.ListeningAudios.Any(e => e.AudioId == id);
        }
    }
}