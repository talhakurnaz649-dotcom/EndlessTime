using EndlessTime.Data.Repositories.Abstract;
using EndlessTime.Model.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace EndlessTime.Web.Controllers;

public class AccountController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult Register() => View();

    // GÜNCELLENDİ: Formdan gelen 'fullName' ve 'password' parametreleri doğrudan eşlendi 🚨
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(User user, string fullName, string password)
    {
        try
        {
            var existingUsers = await _unitOfWork.Users.GetAllAsync();
            if (existingUsers.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError(string.Empty, "Bu e-posta adresi zaten kayıtlı.");
                return View(user);
            }

            // Form kutuları ile veritabanı alanları manuel eşleştirildi 🚨
            user.FullName = fullName;
            user.PasswordHash = password;
            user.Role = "User"; // Varsayılan Müşteri rolü

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Kayıt başarısız: " + ex.Message);
        }
        return View(user);
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

        if (user != null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role), // Admin, Moderator veya User
                new Claim("FullName", user.FullName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, "E-posta veya şifre hatalı.");
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var email = User.Identity.Name;
        var users = await _unitOfWork.Users.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Email == email);
        if (user == null) return NotFound();

        var allOrders = await _unitOfWork.Orders.GetAllAsync();
        var userOrders = allOrders.Where(o => o.UserId == user.Id).OrderByDescending(o => o.OrderDate).ToList();
        ViewBag.UserOrders = userOrders;

        var allMessages = await _unitOfWork.ContactMessages.GetAllAsync();
        var userMessages = allMessages.Where(m => m.Email == email).OrderByDescending(m => m.SentDate).ToList();
        ViewBag.UserMessages = userMessages;

        return View(user);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile(string fullName, string phoneNumber, string address)
    {
        var email = User.Identity.Name;
        var users = await _unitOfWork.Users.GetAllAsync();
        var user = users.FirstOrDefault(u => u.Email == email);
        if (user != null)
        {
            user.FullName = fullName;
            user.PhoneNumber = phoneNumber;
            user.Address = address;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();

            TempData["ProfileSuccess"] = "Profil bilgileriniz başarıyla güncellendi.";
        }
        return RedirectToAction(nameof(Profile));
    }
}