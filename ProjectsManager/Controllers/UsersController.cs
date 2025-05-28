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
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;

namespace ProjectsManager.Controllers
{
    public class UsersController : Controller
    {
        private readonly ProjectsManagerContext _context;
        private readonly INotyfService _toastNotification;
        private readonly ILogger<UsersController> _logger;
        IWebHostEnvironment _appEnvironment;

        public UsersController(ProjectsManagerContext context, INotyfService toastNotification, ILogger<UsersController> logger, IWebHostEnvironment appEnvironment)
        {
            _context = context;
            _toastNotification = toastNotification;
            _logger = logger;
            _appEnvironment = appEnvironment;
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

            List<SelectListItem> dropDownList = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Admin", Value = "1" },
                new SelectListItem { Text = "Manager", Value = "2" },
                new SelectListItem { Text = "User", Value = "3" }

            };

            ViewData["Role"] = new SelectList(dropDownList, "Text", "Text", user.Role);
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
            List<SelectListItem> dropDownList = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Admin", Value = "1" },
                new SelectListItem { Text = "Manager", Value = "2" },
                new SelectListItem { Text = "User", Value = "3" }

            };

            ViewData["Role"] = new SelectList(dropDownList, "Text", "Text", user.Role);
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

        // GET: Users/Analize
        public IActionResult Analize()
        {
            _toastNotification.Warning("\nОтчет создан в папке Отчеты!\n", 10);

            var tasks = _context.Tasks.Include(t => t.AssignedUser).Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status).Where(s => s.CompletionDate <= DateOnly.FromDateTime(DateTime.Now) && s.Status.Name == "Готова");

            if (tasks == null)
            {
                return NotFound();
            }

            return View(tasks);
        }

        // POST: Users/Analize
        [HttpPost, ActionName("Analize")]
        [ValidateAntiForgeryToken]
        public IActionResult Analize(DateOnly start, DateOnly end, string file)
        {


            Microsoft.Office.Interop.Excel.Application winword = new Microsoft.Office.Interop.Excel.Application()
            {
                //Отобразить Excel
                Visible = true,
                //Количество листов в рабочей книге
                SheetsInNewWorkbook = 1
            };
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();

            //Добавить рабочую книгу
            Workbook workBook = app.Workbooks.Add(Type.Missing);

            //Отключить отображение окон с сообщениями
            app.DisplayAlerts = false;

            //Получаем первый лист документа (счет начинается с 1)
            Worksheet sheet = (Worksheet)app.Worksheets.get_Item(1);

            //Название листа (вкладки снизу).
            sheet.Name = string.Concat("Отчет ", start.ToString("dd.MM.yyyy"), " - ", end.ToString("dd.MM.yyyy"));

            var tasks = _context.Tasks.Include(t => t.AssignedUser).Include(t => t.Priority).Include(t => t.Project).Include(t => t.Status).Where(s => s.CompletionDate <= end && s.CompletionDate >= start && s.Status.Name == "Готова");


            sheet.Cells[1, 1] = string.Concat("Промежуток времени: ");
            sheet.Cells[1, 2] = string.Concat(start.ToString("dd.MM.yyyy"), " - ", end.ToString("dd.MM.yyyy"));

            //заполнение имен столбцов в excel

            sheet.Cells[3, 1] = "Дата выполнения";
            sheet.Cells[3, 2] = "Ответственный сотрудник";
            sheet.Cells[3, 3] = "";
            sheet.Cells[3, 4] = "";
            sheet.Cells[3, 5] = "Название проекта";

            int j = 4;

            foreach (var task in tasks)
            {

                sheet.Cells[j, 1] = task.CompletionDate.ToString();
                sheet.Cells[j, 2] = task.AssignedUser.LastName;
                sheet.Cells[j, 3] = task.AssignedUser.Name;
                sheet.Cells[j, 4] = task.AssignedUser.Patronymic;
                sheet.Cells[j, 5] = task.Project.Name;
                j++;
            }

            sheet.Cells[j, 5] = "Выполнено задач: " + Convert.ToString(j-1);

            sheet.Columns.AutoFit();

            // и места где его нужно сохранить*/
            app.Application.ActiveWorkbook.SaveAs($"{_appEnvironment.WebRootPath}/Отчеты/{file}.xlsx", Type.Missing,
              Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange,
              Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            app.Quit();

            System.Runtime.InteropServices.Marshal.ReleaseComObject(app);

            winword.Quit();

            _toastNotification.Custom("Отчет создан в папке \"Отчеты\"!", 6, "#602AC3", "fa fa-user");
            return RedirectToAction("Analize");
        }
    }
}
