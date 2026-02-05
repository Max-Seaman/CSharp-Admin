using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CSharp_Admin.Models;
using CSharp_Admin.Data;
using Microsoft.EntityFrameworkCore;

namespace CSharp_Admin.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // Totals
        ViewBag.TotalCompanies = _context.Companies.Count();
        ViewBag.TotalEmployees = _context.Employees.Count();

        // Recent changes: last 5 employees added
        ViewBag.RecentEmployees = _context.Employees
            .Include(e => e.Company)
            .OrderByDescending(e => e.Id) // or CreatedAt if you add timestamps
            .Take(5)
            .ToList();

        // Recent company additions
        ViewBag.RecentCompanies = _context.Companies
            .OrderByDescending(c => c.Id) // or CreatedAt
            .Take(5)
            .ToList();

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
