using EndlessTime.Data.Repositories.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EndlessTime.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class LogController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public LogController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _unitOfWork.Logs.GetAllAsync();
        return View(logs);
    }
}