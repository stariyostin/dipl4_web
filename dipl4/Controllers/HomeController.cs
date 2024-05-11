using dipl4.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
        public IActionResult Catalog()
        {
            return View();
        }
        public IActionResult CreateOrd(int? productId)
        {
            ViewBag.ErrorMessage = TempData["ErrorMessage"]?.ToString();
            // Получите список продуктов из базы данных
            var products = _context.Products.ToList();

            // Передайте список продуктов в представление
            return View(products);
        }

        [HttpPost]
        public IActionResult CreateOrd(string CustomerName, string CustomerContacts, DateTime DeadlineDate, List<int> ProductIds, List<int> Quantities)
        {
            if (!string.IsNullOrEmpty(CustomerName) && !string.IsNullOrEmpty(CustomerContacts) && ProductIds != null && Quantities != null && ProductIds.Count == Quantities.Count)
            {
                // Проверка дедлайна
                if (DeadlineDate.Date < DateTime.Now.Date)
                {
                    TempData["ErrorMessage"] = "Ошибка при создании заказа: указан некорректный дедлайн";
                    return RedirectToAction("CreateOrd", "Home");
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
                        TempData["ErrorMessage"] = $"Ошибка при создании заказа: товар {product.Name} недоступен на складе";
                        return RedirectToAction("CreateOrd", "Home");
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
            else
            {
                TempData["ErrorMessage"] = "Ошибка при создании заказа: заполните все поля корректно";

                // Если данные неверны, возвращаем пользователя на страницу создания заказа с сообщением об ошибке
                return RedirectToAction("CreateOrd", "Home");
            }

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
