using EndlessTime.Data.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace EndlessTime.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")] // Sadece 3. Seviye Admin erişebilir 🚨
public class UserController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public UserController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        return View(users);
    }

    // YENİ: Yetkilendirme Güncelleme POST Eylemi 🚨
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateRole(int userId, string newRole)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user != null)
        {
            user.Role = newRole; // "Admin", "Moderator" veya "User" yapar
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}