using CSharp_Admin.Models;

namespace CSharp_Admin.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        // Ensure DB exists
        context.Database.EnsureCreated();

        // If we already have companies, assume seeding is done
        if (context.Companies.Any())
            return;

        // --- Companies ---

        var companies = new List<Company>
        {
            new Company { Name = "Acme Corp", Email = "info@acme.com", LogoPath = null, Website = "https://acme.com" },
            new Company { Name = "Globex Ltd", Email = "contact@globex.com", LogoPath = null, Website = "https://globex.com" },
            new Company { Name = "Initech", Email = "hello@initech.com", LogoPath = null, Website = "https://initech.com" }
        };

        context.Companies.AddRange(companies);
        context.SaveChanges();

        // --- Employees ---
        var random = new Random();
        var firstNames = new[] { "John", "Jane", "Mike", "Sarah", "Alex", "Emily", "Chris", "Laura" };
        var lastNames  = new[] { "Smith", "Johnson", "Brown", "Taylor", "Anderson", "Thomas" };

        var employees = new List<Employee>();

        for (int i = 0; i < 20; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var company = companies[random.Next(companies.Count)];

            employees.Add(new Employee
            {
                FirstName = firstName,
                LastName = lastName,
                Email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@example.com",
                Phone = GenerateUkPhoneNumber(random),
                CompanyId = company.Id
            });
        }

        context.Employees.AddRange(employees);
        context.SaveChanges();
    }

    private static string GenerateUkPhoneNumber(Random random)
    {
        // UK phone numbers typically start with 07 and have 11 digits
        return $"07{random.Next(100000000, 999999999)}";
    }
}