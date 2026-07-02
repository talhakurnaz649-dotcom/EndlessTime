using EndlessTime.Data.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EndlessTime.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")] // GÜNCELLENDİ: Hem Admin hem Moderatör görebilir ve cevap yazabilir! 🚨
public class MessageController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public MessageController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var messages = await _unitOfWork.ContactMessages.GetAllAsync();
        return View(messages);
    }

    // YENİ: Mesaj Cevaplama POST Eylemi 🚨
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, string replyText)
    {
        var msg = await _unitOfWork.ContactMessages.GetByIdAsync(id);
        if (msg != null && !string.IsNullOrEmpty(replyText))
        {
            msg.ReplyText = replyText;
            msg.IsReplied = true;
            _unitOfWork.ContactMessages.Update(msg);
            await _unitOfWork.SaveAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")] // Mesajları sadece 3. Seviye Admin tamamen silebilir 🚨
    public async Task<IActionResult> Delete(int id)
    {
        var msg = await _unitOfWork.ContactMessages.GetByIdAsync(id);
        if (msg != null)
        {
            _unitOfWork.ContactMessages.Delete(msg);
            await _unitOfWork.SaveAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}