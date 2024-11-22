using Fattoe_Shaheen_HW5.DAL;
using Fattoe_Shaheen_HW5.Models;
using Fattoe_Shaheen_HW5.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace Fattoe_Shaheen_HW5.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrdersController(AppDbContext context, UserManager<AppUser> userManager)
        {

            _context = context;
            _userManager = userManager;
        }

        // GET: Orders
        public IActionResult Index()
        {
            List<Order> orders;

            // If the user is an admin, display all orders
            if (User.IsInRole("Admin"))
            {
                orders = _context.Orders
                                 .Include(o => o.OrderDetails)
                                 .ToList();
            }
            else // If the user is a customer, only display their own orders
            {
                orders = _context.Orders
                                 .Include(o => o.OrderDetails)
                                 .Where(o => o.AppUser.UserName == User.Identity.Name)
                                 .ToList();
            }

            return View(orders);
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("Error", new String[] { "Please specify an order to view!" });
            }

            Order order = await _context.Orders
                                         .Include(o => o.OrderDetails)
                                         .ThenInclude(od => od.Product)
                                         .Include(o => o.AppUser)
                                         .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return View("Error", new String[] { "This order was not found!" });
            }

            if (User.IsInRole("Customer") && order.AppUser.UserName != User.Identity.Name)
            {
                return View("Error", new String[] { "This is not your order! Please only view your own orders." });
            }

            return View(order);
        }


        // GET: Orders/Create
        [Authorize(Roles = "Customer")]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Orders/Create
        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        //create order -- you don't get to tell what user you are -- we get it from the DB
        public async Task<IActionResult> Create([Bind("OrderNotes")] Order order)
        {
            // Find the next order number from the utilities class
            order.OrderNumber = GenerateNextOrderNumber();

            // Set the date of this order
            order.OrderDate = DateTime.Now;

            // Associate the order with the logged-in customer
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return View("Error", new string[] { "User not found. Please log in and try again." });
            }
            order.AppUser = currentUser;

            // Add the order to the database
            _context.Add(order);
            await _context.SaveChangesAsync();

            // Redirect to the action that allows creating order details
            return RedirectToAction("Create", "OrderDetails", new { orderID = order.OrderID });
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("Error", new string[] { "Please specify an order to edit!" });
            }

            // Include related data for OrderDetails and Product
            Order order = await _context.Orders
                                        .Include(o => o.OrderDetails) // Load OrderDetails
                                        .ThenInclude(od => od.Product) // Load associated Product
                                        .Include(o => o.AppUser) // Load AppUser information
                                        .FirstOrDefaultAsync(o => o.OrderID == id);

            if (order == null)
            {
                return View("Error", new string[] { "This order was not found!" });
            }

            // Security check for customers to ensure they can only edit their own orders
            if (User.IsInRole("Customer") && order.AppUser.UserName != User.Identity.Name)
            {
                return View("Error", new string[] { "This is not your order! You can only edit your own orders." });
            }

            return View(order);
        }


        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderID,OrderNumber,OrderDate,OrderNotes")] Order order)
        {
            if (id != order.OrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderID))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderID == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }

        private int GenerateNextOrderNumber()
        {
            // Get the maximum order number in the database and add 1 to it
            int maxOrderNumber = _context.Orders.Any() ? _context.Orders.Max(o => o.OrderNumber) : 0;
            return maxOrderNumber + 1;
        }

        private SelectList GetAllProducts()
        {
            var products = _context.Products.OrderBy(p => p.Name).ToList();
            return new SelectList(products, "ProductID", "ProductName");
        }
    }
}