using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectsManager.Models;

namespace ProjectsManager.Controllers
{
    public class TeamsProjectsController : Controller
    {
        private readonly ProjectsManagerContext _context;

        public TeamsProjectsController(ProjectsManagerContext context)
        {
            _context = context;
        }

        // GET: TeamsProjects
        public async Task<IActionResult> Index()
        {
            var projectsManagerContext = _context.TeamsProjects.Include(t => t.Project).Include(t => t.Team);
            return View(await projectsManagerContext.ToListAsync());
        }

        // GET: TeamsProjects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamsProject = await _context.TeamsProjects
                .Include(t => t.Project)
                .Include(t => t.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teamsProject == null)
            {
                return NotFound();
            }

            return View(teamsProject);
        }

        // GET: TeamsProjects/Create
        public IActionResult Create()
        {

            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name");
            return View();
        }

        // POST: TeamsProjects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,TeamId")] TeamsProject teamsProject)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teamsProject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", teamsProject.ProjectId);
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", teamsProject.TeamId);
            return View(teamsProject);
        }

        // GET: TeamsProjects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamsProject = await _context.TeamsProjects.FindAsync(id);
            if (teamsProject == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", teamsProject.ProjectId);
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", teamsProject.TeamId);
            return View(teamsProject);
        }

        // POST: TeamsProjects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProjectId,TeamId")] TeamsProject teamsProject)
        {
            if (id != teamsProject.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teamsProject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamsProjectExists(teamsProject.Id))
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
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", teamsProject.ProjectId);
            ViewData["TeamId"] = new SelectList(_context.Teams, "Id", "Name", teamsProject.TeamId);
            return View(teamsProject);
        }

        // GET: TeamsProjects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teamsProject = await _context.TeamsProjects
                .Include(t => t.Project)
                .Include(t => t.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teamsProject == null)
            {
                return NotFound();
            }

            return View(teamsProject);
        }

        // POST: TeamsProjects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teamsProject = await _context.TeamsProjects.FindAsync(id);
            if (teamsProject != null)
            {
                _context.TeamsProjects.Remove(teamsProject);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamsProjectExists(int id)
        {
            return _context.TeamsProjects.Any(e => e.Id == id);
        }
    }
}
