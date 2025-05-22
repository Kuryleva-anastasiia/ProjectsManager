using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectsManager.Models;
using static System.Reflection.Metadata.BlobBuilder;

namespace ProjectsManager.Controllers
{
    public class TasksController : Controller
    {
        private readonly ProjectsManagerContext _context;

        public TasksController(ProjectsManagerContext context)
        {
            _context = context;
        }

        // GET: Tasks
        public async Task<IActionResult> Index(string? sort)
        {
            IQueryable<Models.Task>? tasks = _context.Tasks.Include(t => t.AssignedUser).Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status);

            // сортировка
            switch (sort)
            {
                case "Личные":
                    tasks = tasks.Where(s => s.AssignedUser.Login == User.Identity.Name);
                    break;
                case "По приоритету":
                    tasks = tasks.OrderBy(s => s.PriorityId);
                    break;
                default:
                    tasks = tasks.OrderBy(s => s.Id);
                    break;
            }

            List<string> sortList = new List<string>() { "Все", "Личные", "По приоритету" };



            if (sort != null)
            {
                ViewData["sort"] = new SelectList(sortList, sort);
            }
            else { ViewData["sort"] = new SelectList(sortList, "Все"); }

            return View(await tasks.ToListAsync());
        }

        // GET: Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.AssignedUser)
                .Include(t => t.Priority)
                .Include(t => t.Project)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Tasks/Create
        public IActionResult Create()
        {
            var projectsManagerContext = _context.Users;
            var users = projectsManagerContext.ToList();

            var dropDownList = users.Select(x => new {
                Id = x.Id,
                Name = x.LastName.ToString() + " " + x.Name.ToString() + " " + x.Patronymic.ToString()
            }).ToList();

            ViewData["AssignedUserId"] = new SelectList(dropDownList, "Id", "Name");
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ProjectId,AssignedUserId,StatusId,PriorityId,CreationDate,CompletionDate")] Models.Task task)
        {
            if (ModelState.IsValid)
            {
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var projectsManagerContext = _context.Users;
            var users = projectsManagerContext.ToList();

            var dropDownList = users.Select(x => new {
                Id = x.Id,
                Name = x.LastName.ToString() + " " + x.Name.ToString() + " " + x.Patronymic.ToString()
            }).ToList();

            ViewData["AssignedUserId"] = new SelectList(dropDownList, "Id", "Name", task.AssignedUserId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", task.PriorityId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", task.ProjectId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", task.StatusId);

            return View(task);
        }

        // GET: Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            var projectsManagerContext = _context.Users;
            var users = projectsManagerContext.ToList();

            var dropDownList = users.Select(x => new {
                Id = x.Id,
                Name = x.LastName.ToString() + " " + x.Name.ToString() + " " + x.Patronymic.ToString()
            }).ToList();

            ViewData["AssignedUserId"] = new SelectList(dropDownList, "Id", "Name", task.AssignedUserId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", task.PriorityId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", task.ProjectId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", task.StatusId);

            return View(task);
        }

        // POST: Tasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,ProjectId,AssignedUserId,StatusId,PriorityId,CreationDate,CompletionDate")] Models.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
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

            var projectsManagerContext = _context.Users;
            var users = projectsManagerContext.ToList();

            var dropDownList = users.Select(x => new {
                Id = x.Id,
                Name = x.LastName.ToString() + " " + x.Name.ToString() + " " + x.Patronymic.ToString()
            }).ToList();

            ViewData["AssignedUserId"] = new SelectList(dropDownList, "Id", "Name", task.AssignedUserId);
            ViewData["PriorityId"] = new SelectList(_context.Priorities, "Id", "Name", task.PriorityId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", task.ProjectId);
            ViewData["StatusId"] = new SelectList(_context.Statuses, "Id", "Name", task.StatusId);

            return View(task);
        }

        // GET: Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .Include(t => t.AssignedUser)
                .Include(t => t.Priority)
                .Include(t => t.Project)
                .Include(t => t.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
