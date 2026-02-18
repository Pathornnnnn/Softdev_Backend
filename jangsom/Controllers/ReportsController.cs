using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using jangsom.Models;
using Microsoft.EntityFrameworkCore;
using jangsom.Data;

[ApiController]
[Route("api/[controller]")]
[Authorize] // บังคับว่าต้อง Login (แนบ Token) ถึงจะใช้ได้
public class ReportsController : ControllerBase
{
    private readonly MyDbContext _context; // สมมติว่าใช้ EF Core

    public ReportsController(MyDbContext context)
    {
        _context = context;
    }

    // ---------------------------------------------------------
    // 1. หน้าโฮม (GET All Reports)
    // ---------------------------------------------------------
    [HttpGet]
    public async Task<IActionResult> GetAllReports([FromQuery] string? search, [FromQuery] string? status)
    {
        // Logic: ดึงข้อมูลจาก table 'reports'
        var query = _context.Reports.AsQueryable();

        // Filter ตาม Search หรือ Status
        if (!string.IsNullOrEmpty(search))
            query = query.Where(r => r.Title.Contains(search));
        
        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.Status == status);

        // Map ข้อมูลไปใส่ DTO เพื่อส่งกลับหน้าบ้าน
        var results = query.Include(r => r.Location).Select(r => new ReportResponseDto
        {
            ReportId = r.ReportId,
            Title = r.Title,
            Description = r.Description,
            ImageBeforeUrl = r.ImageBeforeUrl,
            LocationId = r.LocationId,
            AssetId = r.AssetId,
            ReporterId = r.ReporterId,
            TechnicianId = r.TechnicianId,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            LocationName = r.Location.LocationName
        }).ToList();

        return Ok(results);
    }

    // ---------------------------------------------------------
    // 2. หน้าแจ้งซ่อมของฉัน (GET My History)
    // ---------------------------------------------------------
    [HttpGet("me")]
    public async Task<IActionResult> GetMyReports()
    {
        // ดึง User ID จาก Token (ที่ React ส่งมา)
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var myReports = _context.Reports
            .Where(r => r.ReporterId == userId) // *** กรองเฉพาะของฉัน ***
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReportResponseDto {
                ReportId = r.ReportId,
                Title = r.Title,
                Status = r.Status,
                ImageBeforeUrl = r.ImageBeforeUrl,
                CreatedAt = r.CreatedAt
            })
            .ToList();

        return Ok(myReports);
    }

    // ---------------------------------------------------------
    // 3. ฟอร์มแจ้งซ่อม (Create Report)
    // ---------------------------------------------------------
    [HttpPost]
    public async Task<IActionResult> CreateReport([FromBody] CreateReportDto request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        // สร้าง Object เพื่อบันทึกลง Table 'reports'
        var newReport = new Report
        {
            ReportId = Guid.NewGuid(),
            ReporterId = userId,
            Title = request.Title,
            Description = request.Description,
            ImageBeforeUrl = request.ImageBeforeUrl,
            LocationId = request.LocationId,
            AssetId = request.AssetId,
            Status = "pending",
            CreatedAt = DateTime.UtcNow
        };

        _context.Reports.Add(newReport);
        await _context.SaveChangesAsync();

        return Ok(new { message = "แจ้งซ่อมสำเร็จ", id = newReport.ReportId });
    }
}