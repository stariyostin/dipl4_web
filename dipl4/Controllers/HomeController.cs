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
            ViewBag.ProductId = productId; // �������� productId � ViewBag

            // �������� ������ ��������� �� ���� ������
            var products = _context.Products.ToList();

            // ��������� ������ ��������� � �������������
            return View(products);
        }

        [HttpPost]
        public IActionResult CreateOrd(string CustomerName, string CustomerContacts, DateTime DeadlineDate, List<int> ProductIds, List<int> Quantities)
        {
            // ���������� ��������� ������ � TempData ��� �� ���������� ����������� � ������ ������
            TempData["CustomerName"] = CustomerName;
            TempData["CustomerContacts"] = CustomerContacts;
            TempData["DeadlineDate"] = DeadlineDate;
            TempData["ProductIds"] = ProductIds;
            TempData["Quantities"] = Quantities;

            if (string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(CustomerContacts) || ProductIds == null || Quantities == null)
            {
                TempData["ErrorMessage"] = "������ ��� �������� ������: ��������� ��� ���� ���������";
                return RedirectToAction("CreateOrd");
            }

            // �������� ����� �����
            if (string.IsNullOrWhiteSpace(CustomerName))
            {
                TempData["ErrorMessage"] = "������ ��� �������� ������: ��������� ���";
                return RedirectToAction("CreateOrd");
            }

            // �������� �� ��� �������
            if (Regex.IsMatch(CustomerName, @"^\s*[^a-zA-Z�-��-�]+\s*$"))
            {
                TempData["ErrorMessage"] = "������ ��� �������� ������: ��� ������� �� ����� �������� ������ �� ����������� ��������, ���� ��� ��������";
                return RedirectToAction("CreateOrd");
            }

            // �������� ����� ���������
            if (string.IsNullOrWhiteSpace(CustomerContacts))
            {
                TempData["ErrorMessage"] = "������ ��� �������� ������: ��������� ���������� ������";
                return RedirectToAction("CreateOrd");
            }

            // �������� �� �������� �������
            if (Regex.IsMatch(CustomerContacts, @"^\s*[^a-zA-Z�-��-�0-9]+\s*$"))
            {
                TempData["ErrorMessage"] = "������ ��� �������� ������: �������� ������� �� ����� ��������� ������ ����������� �������";
                return RedirectToAction("CreateOrd");
            }

            // �������� �� ������� ������� � ������
            if (ProductIds == null || ProductIds.Count == 0)
            {
                TempData["ErrorMessage"] = "������ ��� �������� ������: �� ������� �� ������ ������";
                return RedirectToAction("CreateOrd");
            }

            // �������� �� ���������� ������� � ������
            for (int i = 0; i < ProductIds.Count; i++)
            {
                if (Quantities == null || Quantities.Count <= i || Quantities[i] <= 0)
                {
                    TempData["ErrorMessage"] = "������ ��� �������� ������: �� ������� ���������� ��� ������ ��� ���������� �������";
                    return RedirectToAction("CreateOrd");
                }
            }

            // �������� ��������
            if (DeadlineDate.Date < DateTime.Now.Date)
            {
                TempData["ErrorMessage"] = "������ ��� �������� ������: ������ ������������ �������";
                return RedirectToAction("CreateOrd");
            }

            // �������� �� ������� ������������� ������
            var Existuser = _context.Users.FirstOrDefault(u => u.ClientName == CustomerName && u.ClientContact == CustomerContacts);
            if (Existuser != null)
            {
                var existingOrder = _context.Orders.FirstOrDefault(o =>
                    o.UserId == Existuser.Iduser &&
                    o.DeadLine.Date == DeadlineDate.Date &&
                    o.StatusId != 2 && o.StatusId != 5);

                if (existingOrder != null)
                {
                    TempData["ErrorMessage"] = "������ ��� �������� ������: ����� ����� ��� ����������, ���� �� ������ �������� ����� ��������� �����, ��������� � ������������ ��������";
                    return RedirectToAction("CreateOrd");
                }
            }

            // �������� �� ������������ ������� � ������
            if (ProductIds.Distinct().Count() != ProductIds.Count)
            {
                TempData["ErrorMessage"] = "������ ��� �������� ������: � ������ ���� ������������� ������";
                return RedirectToAction("CreateOrd");
            }

            // �������� ���������� ������ � ������
            for (int i = 0; i < ProductIds.Count; i++)
            {
                var productId = ProductIds[i];
                var quantity = Quantities[i];

                // ��������� ������� ������ �� ������
                var product = _context.Products.FirstOrDefault(p => p.Idproduct == productId);
                if (product == null || Convert.ToInt32(product.TotalAmount) < quantity)
                {
                    TempData["ErrorMessage"] = $"������ ��� �������� ������: ����� {product?.Name ?? productId.ToString()} � ���������� {quantity} ���������� �� ������";
                    return RedirectToAction("CreateOrd");
                }
            }

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

            TempData["SuccessMessage"] = "����� ������ �������";
            return RedirectToAction("Main", "Home"); // �������������� ������������ �� ������� �������� ����� ��������� �������� ������
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
