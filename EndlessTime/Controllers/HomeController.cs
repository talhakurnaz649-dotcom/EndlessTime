using EndlessTime.Data.Repositories.Abstract;
using EndlessTime.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EndlessTime.Web.Controllers;

public class HomeController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _unitOfWork.Products.GetProductsWithCategoriesAsync();
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null) return NotFound();

        string country = "İsviçre (İthal)";
        if (product.Id == 3 || product.Id == 11) country = "Japonya (İthal)";
        else if (product.Id == 4 || product.Id == 12) country = "Almanya (İthal)";
        else if (product.Id == 8) country = "Amerika Birleşik Devletleri (İthal)";

        ViewBag.OriginCountry = country;
        return View(product);
    }

    public IActionResult About()
    {
        return View();
    }

    // GÜNCELLENDİ: Bize Ulaşın sayfasına giriş şartı konuldu ve bilgiler otomatik aktarıldı 🚨
    [HttpGet]
    [Authorize] // Giriş yapmayan kullanıcıyı otomatik /Account/Login'e atar 🚨
    public async Task<IActionResult> Contact()
    {
        var userEmail = User.Identity.Name;
        var users = await _unitOfWork.Users.GetAllAsync();
        var currentUser = users.FirstOrDefault(u => u.Email == userEmail);

        ViewBag.UserFullName = currentUser?.FullName ?? "";
        ViewBag.UserEmail = userEmail;

        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactMessage model)
    {
        var userEmail = User.Identity.Name;
        var users = await _unitOfWork.Users.GetAllAsync();
        var currentUser = users.FirstOrDefault(u => u.Email == userEmail);

        if (ModelState.IsValid)
        {
            // Kullanıcı verileri dışarıdan manipüle edilemesin diye arka planda eşitlenir 🚨
            model.Email = userEmail;
            model.Name = currentUser?.FullName ?? "Bilinmeyen Üye";
            model.SentDate = DateTime.Now;
            model.IsReplied = false;

            await _unitOfWork.ContactMessages.AddAsync(model);
            await _unitOfWork.SaveAsync();

            TempData["MessageSuccess"] = "Mesajınız başarıyla iletildi. En kısa sürede dönüş yapacağız!";
            return RedirectToAction(nameof(Contact));
        }

        ViewBag.UserFullName = currentUser?.FullName ?? "";
        ViewBag.UserEmail = userEmail;
        return View(model);
    }
}