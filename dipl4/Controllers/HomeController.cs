using dipl4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace dipl4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PapirusContext _context; // Добавьте контекст базы данных в ваш контроллер

        public HomeController(ILogger<HomeController> logger, PapirusContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}
        public IActionResult Main()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"]?.ToString();
            return View();
        }

        [HttpGet]
        public JsonResult AutocompleteSearch(string term)
        {
            var products = _context.Products
                .Where(p => p.Name.Contains(term))
                .Select(p => new { label = p.Name, value = p.Idproduct })
                .ToList();

            return Json(products);
        }

        public IActionResult Catalog(string searchString)
        {
            var products = from p in _context.Products
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Name.Contains(searchString));
            }

            ViewData["CurrentFilter"] = searchString;
            return View(products.ToList());
        }

        public IActionResult CreateOrd(int? productId)
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"]?.ToString();
            ViewBag.CustomerName = TempData["CustomerName"]?.ToString();
            ViewBag.CustomerContacts = TempData["CustomerContacts"]?.ToString();
            ViewBag.DeadlineDate = TempData["DeadlineDate"];
            ViewBag.ProductIds = TempData["ProductIds"] as List<int>;
            ViewBag.Quantities = TempData["Quantities"] as List<int>;
            ViewBag.ProductId = productId; // Передаем productId в ViewBag

            // Получите список продуктов из базы данных
            var products = _context.Products.ToList();

            // Передайте список продуктов в представление
            return View(products);
        }

        [HttpPost]
        public IActionResult CreateOrd(string CustomerName, string CustomerContacts, DateTime DeadlineDate, List<int> ProductIds, List<int> Quantities)
        {
            // Сохранение введенных данных в TempData для их повторного отображения в случае ошибки
            TempData["CustomerName"] = CustomerName;
            TempData["CustomerContacts"] = CustomerContacts;
            TempData["DeadlineDate"] = DeadlineDate;
            TempData["ProductIds"] = ProductIds;
            TempData["Quantities"] = Quantities;

            if (string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(CustomerContacts) || ProductIds == null || Quantities == null)
            {
                TempData["ErrorMessage"] = "Ошибка при создании заказа: заполните все поля корректно";
                return RedirectToAction("CreateOrd");
            }

            // Проверка ввода имени
            if (string.IsNullOrWhiteSpace(CustomerName))
            {
                TempData["ErrorMessage"] = "Ошибка при создании заказа: заполните имя";
                return RedirectToAction("CreateOrd");
            }

            // Проверка на имя клиента
            if (Regex.IsMatch(CustomerName, @"^\s*[^a-zA-Zа-яА-Я]+\s*$"))
            {
                TempData["ErrorMessage"] = "Ошибка при создании заказа: Имя клиента не может состоять только из специальных символов, цифр или пробелов";
                return RedirectToAction("CreateOrd");
            }

            // Проверка ввода контактов
            if (string.IsNullOrWhiteSpace(CustomerContacts))
            {
                TempData["ErrorMessage"] = "Ошибка при создании заказа: заполните контактные данные";
                return RedirectToAction("CreateOrd");
            }

            // Проверка на контакты клиента
            if (Regex.IsMatch(CustomerContacts, @"^\s*[^a-zA-Zа-яА-Я0-9]+\s*$"))
            {
                TempData["ErrorMessage"] = "Ошибка при создании заказа: Контакты клиента не может содержать только специальные символы";
                return RedirectToAction("CreateOrd");
            }

            // Проверка на наличие товаров в заказе
            if (ProductIds == null || ProductIds.Count == 0)
            {
                TempData["ErrorMessage"] = "Ошибка при создании заказа: Не выбрано ни одного товара";
                return RedirectToAction("CreateOrd");
            }

            // Проверка на количество товаров в заказе
            for (int i = 0; i < ProductIds.Count; i++)
            {
                if (Quantities == null || Quantities.Count <= i || Quantities[i] <= 0)
                {
                    TempData["ErrorMessage"] = "Ошибка при создании заказа: Не указано количество для одного или нескольких товаров";
                    return RedirectToAction("CreateOrd");
                }
            }

            // Проверка дедлайна
            if (DeadlineDate.Date < DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "Ошибка при создании заказа: указан некорректный дедлайн";
                return RedirectToAction("CreateOrd");
            }

            // Проверка на наличие существующего заказа
            var Existuser = _context.Users.FirstOrDefault(u => u.ClientName == CustomerName && u.ClientContact == CustomerContacts);
            if (Existuser != null)
            {
                var existingOrder = _context.Orders.FirstOrDefault(o =>
                    o.UserId == Existuser.Iduser &&
                    o.DeadLine.Date == DeadlineDate.Date &&
                    o.StatusId != 2 && o.StatusId != 5);

                if (existingOrder != null)
                {
                    TempData["ErrorMessage"] = "Ошибка при создании заказа: Такой заказ уже существует, если вы хотите изменить ранее созданный заказ, свяжитесь с менеджерским составом";
                    return RedirectToAction("CreateOrd");
                }
            }

            // Проверка на дублирование товаров в заказе
            if (ProductIds.Distinct().Count() != ProductIds.Count)
            {
                TempData["ErrorMessage"] = "Ошибка при создании заказа: В заказе есть дублирующиеся товары";
                return RedirectToAction("CreateOrd");
            }

            // Проверка количество товара в заказе
            for (int i = 0; i < ProductIds.Count; i++)
            {
                var productId = ProductIds[i];
                var quantity = Quantities[i];

                // Проверяем наличие товара на складе
                var product = _context.Products.FirstOrDefault(p => p.Idproduct == productId);
                if (product == null || Convert.ToInt32(product.TotalAmount) < quantity)
                {
                    TempData["ErrorMessage"] = $"Ошибка при создании заказа: товар {product?.Name ?? productId.ToString()} в количестве {quantity} недоступен на складе";
                    return RedirectToAction("CreateOrd");
                }
            }

            // Проверяем наличие пользователя
            var user = _context.Users.FirstOrDefault(u => u.ClientName == CustomerName && u.ClientContact == CustomerContacts);

            // Если пользователь не найден, создаем нового
            if (user == null)
            {
                user = new User
                {
                    ClientName = CustomerName,
                    ClientContact = CustomerContacts
                    // Другие поля, если необходимо
                };
                _context.Users.Add(user);
                _context.SaveChanges();
            }

            // Создаем новый заказ
            var order = new Order
            {
                // Заполняем данные заказа
                DateOfCreate = DateTime.Now,
                DeadLine = DeadlineDate,
                UserId = user.Iduser,
                StatusId = 1,
                // Дополнительные данные о заказе, если необходимо
            };

            // Сохраняем заказ в базе данных
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Добавляем товары в заказ
            for (int i = 0; i < ProductIds.Count; i++)
            {
                var orderProduct = new OrderProduct
                {
                    OrderId = order.Idorder,
                    ProductId = ProductIds[i],
                    Amount = Convert.ToString(Quantities[i]),
                    // Дополнительные данные о товаре в заказе, если необходимо
                };
                _context.OrderProducts.Add(orderProduct);
                _context.SaveChanges();
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Заказ создан успешно";
            return RedirectToAction("Main", "Home"); // Перенаправляем пользователя на главную страницу после успешного создания заказа
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
