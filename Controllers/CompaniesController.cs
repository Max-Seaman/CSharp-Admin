using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CSharp_Admin.Data;
using CSharp_Admin.Models;

namespace CSharp_Admin.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Companies.ToListAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .Include(c => c.Employees)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Logo,Website")] Company company)
        {
            if (ModelState.IsValid)
            {
                string? ext = null;
                if (company.Logo != null)
                {
                    ext = Path.GetExtension(company.Logo.FileName).ToLowerInvariant();

                    if (!IsValidImage(company.Logo, out var errorMessage, ext))
                    {
                        ModelState.AddModelError("Logo", errorMessage);
                        return View(company);
                    }

                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
                    Directory.CreateDirectory(uploadsFolder);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await company.Logo.CopyToAsync(stream);
                    }

                    company.LogoPath = Path.Combine("img", fileName).Replace("\\", "/");
                }

                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = company.Id });
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,LogoPath,Website,Logo")] Company company)
        {
            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // handle new logo upload
                    if (company.Logo != null)
                    {
                        var ext = Path.GetExtension(company.Logo.FileName).ToLowerInvariant();
                        if (!IsValidImage(company.Logo, out var err, ext))
                        {
                            ModelState.AddModelError("Logo", err);
                            return View(company);
                        }

                        var fileName = $"{Guid.NewGuid()}{ext}";
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img");
                        Directory.CreateDirectory(uploadsFolder);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await company.Logo.CopyToAsync(stream);
                        }

                        // delete old file if exists
                        if (!string.IsNullOrEmpty(company.LogoPath))
                        {
                            try
                            {
                                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", company.LogoPath.Replace('/', '\\'));
                                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                            }
                            catch { }
                        }

                        company.LogoPath = Path.Combine("img", fileName).Replace("\\", "/");
                    }

                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company != null)
            {
                _context.Companies.Remove(company);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.Id == id);
        }

        // Validate uploaded image (simple checks)
        private bool IsValidImage(Microsoft.AspNetCore.Http.IFormFile file, out string errorMessage, string ext)
        {
            errorMessage = string.Empty;
            var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!allowedExt.Contains(ext))
            {
                errorMessage = "Unsupported image format. Allowed: jpg, jpeg, png, gif, webp.";
                return false;
            }

            // size limit 5 MB
            const long maxBytes = 5 * 1024 * 1024;
            if (file.Length <= 0 || file.Length > maxBytes)
            {
                errorMessage = "Image size must be between 1 byte and 5 MB.";
                return false;
            }

            return true;
        }
    }
}
