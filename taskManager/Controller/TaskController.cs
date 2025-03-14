using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskManager.Data;
using taskManager.Models;

[Route("api/tasks")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly TaskDbContext _context;
    public TaskController(TaskDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItems>>> GetTasks() =>
        await _context.TaskTable.ToListAsync();

    [HttpGet("today")]
    public async Task<IActionResult> GetTasksForToday()
    {
        var today = DateTime.UtcNow.Date;
        var tasksDueToday = await _context.TaskTable
            .Where(task => task.DueDate.Date == today)
            .ToListAsync();
        return Ok(tasksDueToday);
    }



    [HttpGet("{id}")]
    public async Task<ActionResult<TaskItems>> GetTask(int id)
    {
        var task = await _context.TaskTable.FindAsync(id);
        return task == null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskItems>> CreateTask(TaskItems task)
    {
        _context.TaskTable.Add(task);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItems updatedTask)
    {
        var existingTask = await _context.TaskTable.FindAsync(id);
        if (existingTask == null) return NotFound();

        existingTask.Title = updatedTask.Title;
        existingTask.Description = updatedTask.Description;
        existingTask.DueDate = updatedTask.DueDate;
        existingTask.IsCompleted = updatedTask.IsCompleted;
        existingTask.Priority = updatedTask.Priority;

        await _context.SaveChangesAsync();
        return NoContent();
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.TaskTable.FindAsync(id);
        if (task == null) return NotFound();
        
        _context.TaskTable.Remove(task);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchTasks([FromQuery] string searchQuery)
    {
        var today = DateTime.Today;
        var tasksQuery = _context.TaskTable.AsQueryable();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            tasksQuery = tasksQuery.Where(task => task.Title.Contains(searchQuery) || task.Description.Contains(searchQuery));
        }

        var tasks = await tasksQuery.ToListAsync();
        var todaysTasks = tasks.Where(task => task.DueDate.Date == today).ToList();

        return Ok(new { tasks, todaysTasks });
    }
}
