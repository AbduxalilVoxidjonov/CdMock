using CdMock.Data;
using CdMock.Models.Writing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CdMock.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WritingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public WritingController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Writing
        public async Task<IActionResult> Index()
        {
            var writingTasks = await _context.WritingTasks
                .Include(w => w.Mocks)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();

            return View(writingTasks);
        }

        // GET: Writing/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var writingTask = await _context.WritingTasks
                .Include(w => w.Mocks)
                .FirstOrDefaultAsync(m => m.TaskId == id);

            if (writingTask == null)
            {
                return NotFound();
            }

            return View(writingTask);
        }

        // GET: Writing/Create
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

        // POST: Writing/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WritingTask model, IFormFile? task1Image, IFormFile? task2Image)
        {
            Console.WriteLine($"=== CREATE WRITING TASK ===");
            Console.WriteLine($"Title: {model.Title}");
            Console.WriteLine($"TaskType: {model.TaskType}");
            Console.WriteLine($"MockId: {model.MockId}");
            Console.WriteLine($"Task1Image: {task1Image?.FileName ?? "NULL"}");
            Console.WriteLine($"Task2Image: {task2Image?.FileName ?? "NULL"}");

            // Rasm formatini tekshirish
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            if (task1Image != null)
            {
                var ext1 = Path.GetExtension(task1Image.FileName)?.ToLower();
                if (!allowedExtensions.Contains(ext1))
                {
                    ModelState.AddModelError("task1Image", "Faqat JPG, PNG, GIF, WEBP formatdagi rasmlar qabul qilinadi!");
                }
            }

            if (task2Image != null)
            {
                var ext2 = Path.GetExtension(task2Image.FileName)?.ToLower();
                if (!allowedExtensions.Contains(ext2))
                {
                    ModelState.AddModelError("task2Image", "Faqat JPG, PNG, GIF, WEBP formatdagi rasmlar qabul qilinadi!");
                }
            }

            // Navigation property ni ModelState dan olib tashlash
            ModelState.Remove("Mocks");
            ModelState.Remove("Task1ImagePath");
            ModelState.Remove("Task1ImageName");
            ModelState.Remove("Task2ImagePath");
            ModelState.Remove("Task2ImageName");

            if (ModelState.IsValid)
            {
                try
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "writing");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Task 1 rasmini yuklash
                    if (task1Image != null && task1Image.Length > 0)
                    {
                        var uniqueFileName1 = $"task1_{Guid.NewGuid()}_{task1Image.FileName}";
                        var filePath1 = Path.Combine(uploadsFolder, uniqueFileName1);

                        using (var fileStream = new FileStream(filePath1, FileMode.Create))
                        {
                            await task1Image.CopyToAsync(fileStream);
                        }

                        model.Task1ImagePath = $"/uploads/writing/{uniqueFileName1}";
                        model.Task1ImageName = task1Image.FileName;
                    }

                    // Task 2 rasmini yuklash
                    if (task2Image != null && task2Image.Length > 0)
                    {
                        var uniqueFileName2 = $"task2_{Guid.NewGuid()}_{task2Image.FileName}";
                        var filePath2 = Path.Combine(uploadsFolder, uniqueFileName2);

                        using (var fileStream = new FileStream(filePath2, FileMode.Create))
                        {
                            await task2Image.CopyToAsync(fileStream);
                        }

                        model.Task2ImagePath = $"/uploads/writing/{uniqueFileName2}";
                        model.Task2ImageName = task2Image.FileName;
                    }

                    model.CreatedAt = DateTime.Now;

                    _context.Add(model);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Writing task muvaffaqiyatli yaratildi!";
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

        // GET: Writing/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var writingTask = await _context.WritingTasks.FindAsync(id);
            if (writingTask == null)
            {
                return NotFound();
            }

            var activeMocks = _context.Mocks
                .Where(m => m.IsActive)
                .Select(m => new { m.Id, m.Name })
                .ToList();

            ViewBag.Mocks = new SelectList(activeMocks, "Id", "Name", writingTask.MockId);
            return View(writingTask);
        }

        // POST: Writing/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WritingTask model, IFormFile? task1Image, IFormFile? task2Image)
        {
            if (id != model.TaskId)
            {
                return NotFound();
            }

            ModelState.Remove("Mocks");
            if (task1Image == null)
            {
                ModelState.Remove("Task1ImagePath");
                ModelState.Remove("Task1ImageName");
            }
            if (task2Image == null)
            {
                ModelState.Remove("Task2ImagePath");
                ModelState.Remove("Task2ImageName");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "writing");

                    // Task 1 rasmini yangilash
                    if (task1Image != null && task1Image.Length > 0)
                    {
                        // Eski rasmni o'chirish
                        if (!string.IsNullOrEmpty(model.Task1ImagePath))
                        {
                            var oldPath1 = Path.Combine(_environment.WebRootPath, model.Task1ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath1))
                            {
                                System.IO.File.Delete(oldPath1);
                            }
                        }

                        // Yangi rasmni yuklash
                        var uniqueFileName1 = $"task1_{Guid.NewGuid()}_{task1Image.FileName}";
                        var filePath1 = Path.Combine(uploadsFolder, uniqueFileName1);

                        using (var fileStream = new FileStream(filePath1, FileMode.Create))
                        {
                            await task1Image.CopyToAsync(fileStream);
                        }

                        model.Task1ImagePath = $"/uploads/writing/{uniqueFileName1}";
                        model.Task1ImageName = task1Image.FileName;
                    }

                    // Task 2 rasmini yangilash
                    if (task2Image != null && task2Image.Length > 0)
                    {
                        // Eski rasmni o'chirish
                        if (!string.IsNullOrEmpty(model.Task2ImagePath))
                        {
                            var oldPath2 = Path.Combine(_environment.WebRootPath, model.Task2ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldPath2))
                            {
                                System.IO.File.Delete(oldPath2);
                            }
                        }

                        // Yangi rasmni yuklash
                        var uniqueFileName2 = $"task2_{Guid.NewGuid()}_{task2Image.FileName}";
                        var filePath2 = Path.Combine(uploadsFolder, uniqueFileName2);

                        using (var fileStream = new FileStream(filePath2, FileMode.Create))
                        {
                            await task2Image.CopyToAsync(fileStream);
                        }

                        model.Task2ImagePath = $"/uploads/writing/{uniqueFileName2}";
                        model.Task2ImageName = task2Image.FileName;
                    }

                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Writing task muvaffaqiyatli yangilandi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WritingTaskExists(model.TaskId))
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

        // GET: Writing/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var writingTask = await _context.WritingTasks
                .Include(w => w.Mocks)
                .FirstOrDefaultAsync(m => m.TaskId == id);

            if (writingTask == null)
            {
                return NotFound();
            }

            return View(writingTask);
        }

        // POST: Writing/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var writingTask = await _context.WritingTasks.FindAsync(id);
            if (writingTask != null)
            {
                // Task 1 rasmni o'chirish
                if (!string.IsNullOrEmpty(writingTask.Task1ImagePath))
                {
                    var path1 = Path.Combine(_environment.WebRootPath, writingTask.Task1ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(path1))
                    {
                        System.IO.File.Delete(path1);
                    }
                }

                // Task 2 rasmni o'chirish
                if (!string.IsNullOrEmpty(writingTask.Task2ImagePath))
                {
                    var path2 = Path.Combine(_environment.WebRootPath, writingTask.Task2ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(path2))
                    {
                        System.IO.File.Delete(path2);
                    }
                }

                _context.WritingTasks.Remove(writingTask);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Writing task o'chirildi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool WritingTaskExists(int id)
        {
            return _context.WritingTasks.Any(e => e.TaskId == id);
        }
    }
}