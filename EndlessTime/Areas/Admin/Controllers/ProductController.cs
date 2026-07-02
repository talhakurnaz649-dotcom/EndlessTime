using EndlessTime.Data.Repositories.Abstract;
using EndlessTime.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EndlessTime.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")] // Hem Admin hem Moderatör girebilir
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _env;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment env)
    {
        _unitOfWork = unitOfWork;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _unitOfWork.Products.GetProductsWithCategoriesAsync();

        var orders = await _unitOfWork.Orders.GetAllAsync();
        decimal totalSales = orders.Sum(o => o.TotalAmount);
        ViewBag.TotalSales = totalSales;

        return View(products);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")] // Sadece Seviye 3 Admin yeni saat ekleyebilir
    public async Task<IActionResult> Create()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        ViewBag.CategoriesList = categories.ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Product product, IFormFile imageFile)
    {
        try
        {
            if (imageFile != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
                product.ImageUrl = "/uploads/" + uniqueFileName;
            }
            else
            {
                product.ImageUrl = "/uploads/default-watch.jpg";
            }

            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Ürün ekleme başarısız: " + ex.Message);
        }

        var categories = await _unitOfWork.Categories.GetAllAsync();
        ViewBag.CategoriesList = categories.ToList();
        return View(product);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();

        var categories = await _unitOfWork.Categories.GetAllAsync();
        ViewBag.CategoriesList = categories.ToList();

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product model, IFormFile? imageFile)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();

        try
        {
            product.Name = model.Name;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.Description = model.Description;
            product.CategoryId = model.CategoryId;

            if (imageFile != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }
                product.ImageUrl = "/uploads/" + uniqueFileName;
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Güncelleme başarısız: " + ex.Message);
        }

        var categories = await _unitOfWork.Categories.GetAllAsync();
        ViewBag.CategoriesList = categories.ToList();
        return View(product);
    }

    // ÜRÜNÜ SİL EYLEMİ (Yalnızca Admin Silebilir 🚨)
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                _unitOfWork.Products.Delete(product);
                await _unitOfWork.SaveAsync();
            }
        }
        catch (Exception ex)
        {
            TempData["DeleteError"] = "Saat silinemedi: " + ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}