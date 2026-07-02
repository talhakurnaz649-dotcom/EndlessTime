using EndlessTime.Data.Repositories.Abstract;
using EndlessTime.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EndlessTime.Web.Controllers;

public class CartController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private const string CartCookieName = "EndlessTime_Cart";

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var productIds = GetCartItemsFromCookie();
        var products = new List<Product>();

        foreach (var id in productIds)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null)
            {
                products.Add(product);
            }
        }

        return View(products);
    }

    // Stok Sıfırsa Sepete Ekleme Engellenir 🚨
    public async Task<IActionResult> Add(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product != null && product.Stock > 0) // Stok kontrolü 🚨
        {
            var productIds = GetCartItemsFromCookie();
            productIds.Add(id);
            SaveCartItemsToCookie(productIds);
        }

        return RedirectToAction("Index");
    }

    public IActionResult Remove(int id)
    {
        var productIds = GetCartItemsFromCookie();
        productIds.Remove(id);
        SaveCartItemsToCookie(productIds);

        return RedirectToAction("Index");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        var email = User.Identity.Name;
        var users = await _unitOfWork.Users.GetAllAsync();
        var currentUser = users.FirstOrDefault(u => u.Email == email || u.FullName == email);

        if (currentUser == null) return RedirectToAction("Login", "Account");

        var productIds = GetCartItemsFromCookie();
        var products = new List<Product>();
        foreach (var id in productIds)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product != null) products.Add(product);
        }

        ViewBag.TotalAmount = products.Sum(p => p.Price);

        return View(currentUser);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckoutConfirm(string phoneNumber, string address, string cardNumber, string cardHolderName, string cardExpiry, string cardCvv, decimal totalAmount)
    {
        var email = User.Identity.Name;
        var users = await _unitOfWork.Users.GetAllAsync();
        var currentUser = users.FirstOrDefault(u => u.Email == email || u.FullName == email);

        if (currentUser != null)
        {
            currentUser.PhoneNumber = phoneNumber;
            currentUser.Address = address;
            currentUser.CardNumber = cardNumber;
            currentUser.CardHolderName = cardHolderName;
            currentUser.CardExpiry = cardExpiry;
            currentUser.CardCvv = cardCvv;

            _unitOfWork.Users.Update(currentUser);

            var newOrder = new Order
            {
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                UserId = currentUser.Id
            };
            await _unitOfWork.Orders.AddAsync(newOrder);

            var productIds = GetCartItemsFromCookie();
            foreach (var id in productIds)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(id);
                if (product != null)
                {
                    if (product.Stock > 0)
                    {
                        product.Stock -= 1;
                        _unitOfWork.Products.Update(product);
                    }
                }
            }

            await _unitOfWork.SaveAsync();
            Response.Cookies.Delete(CartCookieName);
        }

        return RedirectToAction("Success");
    }

    public IActionResult Success() => View();

    private List<int> GetCartItemsFromCookie()
    {
        var cookie = Request.Cookies[CartCookieName];
        if (string.IsNullOrEmpty(cookie)) return new List<int>();
        return cookie.Split(',').Select(int.Parse).ToList();
    }

    private void SaveCartItemsToCookie(List<int> items)
    {
        var cookieValue = string.Join(",", items);
        Response.Cookies.Append(CartCookieName, cookieValue, new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(7),
            HttpOnly = true
        });
    }
}