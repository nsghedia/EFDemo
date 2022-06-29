using EFDataAccesslibrary.DataAccess;
using EFDataAccesslibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EFDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly PeopleContext _peopleContext;

        public IndexModel(ILogger<IndexModel> logger, PeopleContext peopleContext)
        {
            _logger = logger;
            _peopleContext = peopleContext;
        }

        public async Task OnGet()
        {
            try
            {
                await LoadSampleData();

                var People = _peopleContext.People;
                var allData = People
                    .Include(a => a.Addresses)
                    .Include(e => e.EmailAddresses)
                    //.Where(e => ApprovedAge(e.Age))
                    .Where(e => e.Age >= 18 && e.Age <= 60)
                    .ToList();

                if (allData != null && allData.Count > 0) { }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                await Task.CompletedTask;
            }
        }

        private bool ApprovedAge(int age)
        {
            return age >= 18 && age <= 60;
        }

        private async Task LoadSampleData()
        {
            if (_peopleContext.People.Count() <= 0)
            {
                string File = System.IO.File.ReadAllText("PeopleData.json");
                if (!string.IsNullOrEmpty(File))
                {
                    var people = JsonSerializer.Deserialize<List<Person>>(File);
                    if (people != null && people.Count > 0)
                    {
                        await _peopleContext.AddRangeAsync(people);
                        _peopleContext.SaveChanges();
                    }
                }
            }
        }

    }
}