using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using jangsom.Models;
using jangsom.Data;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly MyDbContext _context;

    public NotificationsController(MyDbContext context)
    {
        _context = context;
    }

    // ---------------------------------------------------------
    // ดึงรายการแจ้งเตือน (GET Notifications)
    // ---------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) return Unauthorized(); // Or handle appropriately
        var userId = Guid.Parse(claim.Value);

        var notifs = _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead) // เฉพาะของ User คนนี้ และยังไม่ได้อ่าน
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationResponseDto
            {
                NotificationId = n.NotificationId,
                Title = n.Title,
                IsRead = n.IsRead,
                Type = n.Type,
                CreatedAt = n.CreatedAt
            })
            .ToList();

        return Ok(notifs);
    }

    // ---------------------------------------------------------
    // กดอ่านแล้ว (Mark as Read)
    // ---------------------------------------------------------
    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var notif = _context.Notifications.Find(id);
        if (notif == null) return NotFound();

        notif.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok();
    }
}