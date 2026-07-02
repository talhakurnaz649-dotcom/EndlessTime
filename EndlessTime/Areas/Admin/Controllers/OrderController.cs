using ClosedXML.Excel;
using EndlessTime.Data.Repositories.Abstract;
using EndlessTime.Model.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EndlessTime.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,Moderator")] // Hem Admin (3. Seviye) hem Moderatör (2. Seviye) erişebilir 🚨
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public OrderController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // Sipariş Listesi
    public async Task<IActionResult> Index()
    {
        var orders = await _unitOfWork.Orders.GetAllAsync();
        return View(orders);
    }

    // Yeni Sipariş Ekleme Sayfası (GET)
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        ViewBag.Users = new SelectList(users, "Id", "FullName");
        return View();
    }

    // Yeni Sipariş Ekleme Sayfası (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Order order)
    {
        try
        {
            order.OrderDate = DateTime.Now;
            order.Status = "Sipariş Verildi"; // Varsayılan durum

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Sipariş eklenemedi: " + ex.Message);
        }

        var users = await _unitOfWork.Users.GetAllAsync();
        ViewBag.Users = new SelectList(users, "Id", "FullName", order.UserId);
        return View(order);
    }

    // YENİ: Sipariş/Kargo Durumunu Güncelleme POST Eylemi 🚨
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int orderId, string newStatus)
    {
        try
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                _unitOfWork.Orders.Update(order);
                await _unitOfWork.SaveAsync();
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Durum güncellenirken hata oluştu: " + ex.Message);
        }

        return RedirectToAction(nameof(Index));
    }

    // Excel İndirme Servisi
    public async Task<IActionResult> ExportToExcel()
    {
        var orders = await _unitOfWork.Orders.GetAllAsync();
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Siparişler");
            worksheet.Cell(1, 1).Value = "Sipariş No";
            worksheet.Cell(1, 2).Value = "Sipariş Tarihi";
            worksheet.Cell(1, 3).Value = "Durum";
            worksheet.Cell(1, 4).Value = "Toplam Tutar (TL)";

            var headerStyle = worksheet.Range("A1:D1");
            headerStyle.Style.Font.Bold = true;
            headerStyle.Style.Fill.BackgroundColor = XLColor.FromHtml("#1C1C1C");
            headerStyle.Style.Font.FontColor = XLColor.FromHtml("#D4AF37");

            int row = 2;
            foreach (var order in orders)
            {
                worksheet.Cell(row, 1).Value = order.Id;
                worksheet.Cell(row, 2).Value = order.OrderDate.ToString("dd.MM.yyyy HH:mm");
                worksheet.Cell(row, 3).Value = order.Status;
                worksheet.Cell(row, 4).Value = order.TotalAmount;
                row++;
            }
            worksheet.Columns().AdjustToContents();

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EndlessTime_Siparisler.xlsx");
            }
        }
    }

    // PDF İndirme Servisi
    public async Task<IActionResult> ExportToPdf()
    {
        var orders = await _unitOfWork.Orders.GetAllAsync();
        QuestPDF.Settings.License = LicenseType.Community;

        var pdfBytes = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Helvetica"));
                page.Header().Text("ENDLESSTIME - SİPARİŞ RAPORU").SemiBold().FontSize(18).FontColor("#D4AF37");
                page.Content().PaddingVertical(1, Unit.Centimetre).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(3);
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Background("#1C1C1C").Padding(5).Text("Sipariş No").Bold().FontColor("#D4AF37");
                        header.Cell().Background("#1C1C1C").Padding(5).Text("Sipariş Tarihi").Bold().FontColor("#D4AF37");
                        header.Cell().Background("#1C1C1C").Padding(5).Text("Durum").Bold().FontColor("#D4AF37");
                        header.Cell().Background("#1C1C1C").Padding(5).Text("Toplam Tutar").Bold().FontColor("#D4AF37");
                    });

                    foreach (var order in orders)
                    {
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(order.Id.ToString());
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(order.OrderDate.ToString("dd.MM.yyyy HH:mm"));
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(order.Status);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text($"{order.TotalAmount:N0} TL");
                    }
                });
                page.Footer().AlignCenter().Text(x => { x.Span("Sayfa "); x.CurrentPageNumber(); });
            });
        }).GeneratePdf();

        return File(pdfBytes, "application/pdf", "EndlessTime_Siparis_Raporu.pdf");
    }
}