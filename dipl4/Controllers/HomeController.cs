using dipl4.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace dipl4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PapirusContext _context; // �������� �������� ���� ������ � ��� ����������

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
            return View();
        }
        public IActionResult Catalog()
        {
            return View();
        }
        public IActionResult CreateOrd()
        {
            // �������� ������ ��������� �� ���� ������
            var products = _context.Products.ToList();

            // ��������� ������ ��������� � �������������
            return View(products);
        }

        [HttpPost]
        public IActionResult CreateOrd(string CustomerName, string CustomerContacts, DateTime DeadlineDate, List<int> ProductIds, List<int> Quantities)
        {
            if (!string.IsNullOrEmpty(CustomerName) && !string.IsNullOrEmpty(CustomerContacts) && ProductIds != null && Quantities != null && ProductIds.Count == Quantities.Count)
            {
                // ��������� ������� ������������
                var user = _context.Users.FirstOrDefault(u => u.ClientName == CustomerName && u.ClientContact == CustomerContacts);

                // ���� ������������ �� ������, ������� ������
                if (user == null)
                {
                    user = new User
                    {
                        ClientName = CustomerName,
                        ClientContact = CustomerContacts
                        // ������ ����, ���� ����������
                    };
                    _context.Users.Add(user);
                    _context.SaveChanges();
                }
                // ������� ����� �����
                var order = new Order
                {
                    // ��������� ������ ������
                    DateOfCreate = DateTime.Now,
                    DeadLine = DeadlineDate,
                    UserId = user.Iduser,
                    StatusId = 1,
                    // �������������� ������ � ������, ���� ����������
                };

                // ��������� ����� � ���� ������
                _context.Orders.Add(order);
                _context.SaveChanges();

                // ��������� ������ � �����
                for (int i = 0; i < ProductIds.Count; i++)
                {
                    var orderProduct = new OrderProduct
                    {
                        OrderId = order.Idorder,
                        ProductId = ProductIds[i],
                        Amount = Convert.ToString(Quantities[i]),
                        // �������������� ������ � ������ � ������, ���� ����������
                    };
                    _context.OrderProducts.Add(orderProduct);
                    _context.SaveChanges();
                }

                _context.SaveChanges();

                return RedirectToAction("Index", "Home"); // �������������� ������������ �� ������� �������� ����� ��������� �������� ������
            }

            // ���� ������ �������, ���������� ������������ �� �������� �������� ������ � ���������� �� ������
            return RedirectToAction("CreateOrd", "Home");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
