using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fattoe_Shaheen_HW5.DAL;
using Fattoe_Shaheen_HW5.Models;
using Microsoft.AspNetCore.Authorization;

namespace Fattoe_Shaheen_HW5.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products
                .Include(c => c.Suppliers)
                .FirstOrDefaultAsync(m => m.ProductID == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.AllSuppliers = GetAllSuppliers();
            ViewBag.ProductTypes = new SelectList(Enum.GetValues(typeof(ProductType)));
            return View();
        }

        // POST: Products/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, int[] SelectedSuppliers)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.AllSuppliers = GetAllSuppliers();
                return View(product);
            }

            _context.Add(product);
            await _context.SaveChangesAsync();

            foreach (int supplierID in SelectedSuppliers)
            {
                Supplier dbSupplier = _context.Suppliers.Find(supplierID);
                product.Suppliers.Add(dbSupplier);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _context.Products.Include(c => c.Suppliers)
                                           .FirstOrDefaultAsync(c => c.ProductID == id);
            //var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.AllSuppliers = GetAllSuppliers(product);
            return View(product);
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, int[] SelectedSuppliers)
        {
            if (id != product.ProductID)
            {
                return View("Error", new string[] { "Please try again!" });
            }

            if (ModelState.IsValid == false)
            {
                ViewBag.AllSuppliers = GetAllSuppliers(product);
                ViewBag.ProductTypes = new SelectList(Enum.GetValues(typeof(ProductType)));
                return View(product);
            }

            try
            {
                // Retrieve the existing product from the database with its suppliers
                Product dbProduct = await _context.Products
                    .Include(p => p.Suppliers)
                    .FirstOrDefaultAsync(p => p.ProductID == product.ProductID);

                if (dbProduct == null)
                {
                    return View("Error", new string[] { "Product not found." });
                }

                // Remove suppliers that are no longer selected
                List<Supplier> suppliersToRemove = dbProduct.Suppliers
                    .Where(s => !SelectedSuppliers.Contains(s.SupplierID))
                    .ToList();
                foreach (var supplier in suppliersToRemove)
                {
                    dbProduct.Suppliers.Remove(supplier);
                }
                await _context.SaveChangesAsync();

                // Add suppliers that are newly selected
                foreach (int supplierID in SelectedSuppliers)
                {
                    if (dbProduct.Suppliers.All(s => s.SupplierID != supplierID))
                    {
                        Supplier supplierToAdd = await _context.Suppliers.FindAsync(supplierID);
                        if (supplierToAdd != null)
                        {
                            dbProduct.Suppliers.Add(supplierToAdd);
                        }
                    }
                }

                // Update scalar properties
                dbProduct.Name = product.Name;
                dbProduct.Description = product.Description;
                dbProduct.Price = product.Price;
                dbProduct.ProductType = product.ProductType;

                _context.Products.Update(dbProduct);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return View("Error", new string[] { "There was an error editing this product.", ex.Message });
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
        
        private MultiSelectList GetAllSuppliers()
        {
            List<Supplier> allSuppliers = _context.Suppliers.OrderBy(s => s.SupplierName).ToList();
            return new MultiSelectList(allSuppliers, "SupplierID", "SupplierName");
        }

        // Method to get a list of all suppliers, with selected suppliers pre-marked for the Edit view
        private MultiSelectList GetAllSuppliers(Product product)
        {
            List<Supplier> allSuppliers = _context.Suppliers.OrderBy(s => s.SupplierName).ToList();
            List<int> selectedSupplierIDs = product.Suppliers.Select(sp => sp.SupplierID).ToList();
            return new MultiSelectList(allSuppliers, "SupplierID", "SupplierName", selectedSupplierIDs);
        }
    }
}


















//public async Task<IActionResult> Create([Bind("ProductID,Name,Description,Price,ProductType")] Product product)
//public async Task<IActionResult> Create(Product product, int[] SelectedSuppliers)
//{
//    if (ModelState.IsValid)
//    {
//        _context.Add(product);
//        await _context.SaveChangesAsync();
//        return RedirectToAction(nameof(Index));
//    }
//    ViewBag.AllSuppliers = GetAllSuppliers();
//    return View(product);
//}









//// POST: Products/Edit/5
//[Authorize(Roles = "Admin")]
//[HttpPost]
//[ValidateAntiForgeryToken]
//public async Task<IActionResult> Edit(int id, [Bind("ProductID,Name,Description,Price,ProductType")] Product product)
//{
//    if (id != product.ProductID)
//    {
//        return NotFound();
//    }

//    if (ModelState.IsValid)
//    {
//        try
//        {
//            _context.Update(product);
//            await _context.SaveChangesAsync();
//        }
//        catch (DbUpdateConcurrencyException)
//        {
//            if (!ProductExists(product.ProductID))
//            {
//                return NotFound();
//            }
//            else
//            {
//                throw;
//            }
//        }
//        return RedirectToAction(nameof(Index));
//    }
//    ViewBag.AllSuppliers = GetAllSuppliers(product);
//    return View(product);
//}