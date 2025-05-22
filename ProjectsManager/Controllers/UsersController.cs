using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectsManager.Models;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace ProjectsManager.Controllers
{
    public class UsersController : Controller
    {
        private readonly ProjectsManagerContext _context;
        private readonly INotyfService _toastNotification;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ProjectsManagerContext context, INotyfService toastNotification, ILogger<UsersController> logger)
        {
            _context = context;
            _toastNotification = toastNotification;
            _logger = logger;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            List<SelectListItem> dropDownList = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Admin", Value = "1" },
                new SelectListItem { Text = "Manager", Value = "2" },
                new SelectListItem { Text = "User", Value = "3" }

            };

            ViewData["Role"] = new SelectList(dropDownList, "Text", "Text", "User");

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,Name,Patronymic,Phone,Login,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                if (user.Login != null && user.Password != null)
                {
                    var count = _context.Users.Where(u => u.Login == user.Login).Count();
                    if (count == 0)
                    {
                        if (user.Role == null)
                        {
                            user.Role = "User";
                        }
                        try
                        {
                            user.Password = Crypto.Hash(user.Password.ToString(), "SHA-256");

                            _context.Add(user);
                            await _context.SaveChangesAsync();
                            _toastNotification.Success("Вы успешно зарегистрированы!");
                            return Redirect($"~/Users/SignIn/{user.Id}");
                        }
                        catch (Exception ex)
                        {
                            _toastNotification.Error("Ошибка регистрации!\n" + ex.Message);
                        }
                    }
                    else
                    {
                        _toastNotification.Error("Аккаунт с таким логином уже существует!");
                    }
                }
                else
                {
                    _toastNotification.Error("Логин и пароль обязательны для заполнения!");
                }
            }
            List<SelectListItem> dropDownList = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Admin", Value = "1" },
                new SelectListItem { Text = "Manager", Value = "2" },
                new SelectListItem { Text = "User", Value = "3" }

            };

            ViewData["Role"] = new SelectList(dropDownList, "Text", "Text", user.Role);
            return View(user);
        }

        // GET: User/Login
        public IActionResult Login()
        {
            return View();
        }

        private static int fail = 0; // Счетчик неудачных попыток
        private static DateTime? lockoutEndTime = null; // Время окончания блокировки

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Login,Password")] User model)
        {

            if (lockoutEndTime.HasValue && DateTime.Now < lockoutEndTime.Value)
            {
                // Если пользователь заблокирован
                int min = lockoutEndTime.Value.Minute - DateTime.Now.Minute;
                int sec = lockoutEndTime.Value.Second - DateTime.Now.Second;
                int time = min * 60 + sec;
                _toastNotification.Custom("Вход ограничен. Когда уведомление исчезнет, повторный вход будет разрешен", time, "Orange");
                return View();
            }


            if (model.Login != null && model.Password != null)
            {


                var p = Crypto.Hash(model.Password, "SHA-256");



                var user = _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == p);

                if (user.Result != null && user.Result.Role != null)
                {
                    fail = 0;
                    int id = Convert.ToInt32(user.Result.Id);

                    model.Id = id;
                    _toastNotification.Success("Вы успешно вошли в аккаунт!");
                    return Redirect($"~/Users/SignIn/{id}");
                }
                else
                {
                    ++fail;
                    if (fail < 3)
                    {
                        int notFail = 3 - fail;
                        _toastNotification.Error("Аккаунт не найден! Осталось попыток: " + notFail);
                    }
                    else
                    {
                        _toastNotification.Error("Аккаунт не найден!");
                        _toastNotification.Custom("Вход ограничен на 2 минуты. Когда уведомление исчезнет, повторный вход будет разрешен", 120, "Orange");
                        lockoutEndTime = DateTime.Now.AddMinutes(2); // Установка времени блокировки на 2 минуты
                    }
                        
                    return View(model);
                }
            }
            else
            {
                _toastNotification.Error("Введите логин и пароль!");
            }
            return View();
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,Name,Patronymic,Phone,Login,Role")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);

            int count = _context.Projects.Where(u => u.User == user).Count();
            int count2 = _context.TeamMembers.Where(u => u.User == user).Count();

            if (count > 0 || count2 > 0)
            {
                _toastNotification.Error("Не разрешено удаление пользователя, который уже участвует в проектах!");
            }
            else {
                if (user != null)
                {
                    _context.Users.Remove(user);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Delete));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
