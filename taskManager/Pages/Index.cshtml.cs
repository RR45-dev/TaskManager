using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Microsoft.EntityFrameworkCore;
using taskManager.Data;
using taskManager.Models;
using taskManager.Data;
using System.Collections;

namespace taskManager.Pages;

public class IndexModel : PageModel
{
    private readonly TaskDbContext _context;
    public List<TaskItems> TaskTable { get; set; } = new();
    public List<TaskItems> TodaysTaskTable { get; set; } = new();
    public string SearchQuery { get; set; }

    public IndexModel(TaskDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        DateTime today = DateTime.Today;
        TaskTable = await _context.TaskTable.ToListAsync();
        TodaysTaskTable = await _context.TaskTable
            .Where(task => task.DueDate.Date == today)
            .ToListAsync();
    }

    private static readonly List<string> TaskList = new()
        {
            "Complete Project Report",
            "Prepare Presentation",
            "Submit Expense Sheet",
            "Review Code",
            "Fix UI Bugs",
            "Update Documentation"
        };

    public JsonResult OnGetSearch(string term)
    {
        var results = TaskList
            .Where(t => t.Contains(term, System.StringComparison.OrdinalIgnoreCase))
            .ToList();

        return new JsonResult(results);
    }



}
