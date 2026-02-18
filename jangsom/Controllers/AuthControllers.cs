using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using jangsom.Models;
using jangsom.Data;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly MyDbContext _context;
    private readonly Supabase.Client _supabaseClient; // 1. เพิ่มตัวแปร
    public AuthController(MyDbContext context, Supabase.Client supabaseClient)
    {
        _context = context;
        _supabaseClient = supabaseClient; // จับคู่ให้ถูกต้อง
    }

        // ---------------------------------------------------------
        // 3. เพิ่ม API Login ตรงนี้
        // ---------------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            try
            {
                // สั่งให้ Supabase ช่วยเช็ค Email/Password
                var session = await _supabaseClient.Auth.SignIn(request.Email, request.Password);

                if (session?.User != null && !string.IsNullOrEmpty(session.AccessToken))
                {
                    return Ok(new 
                    { 
                        token = session.AccessToken, // นี่คือ Token ที่เอาไปไขกุญแจ!
                        userId = session.User.Id,
                        email = session.User.Email
                    });
                }
                
                return Unauthorized("Login failed");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

    // เรียก API นี้หลังจาก Login ที่หน้าบ้านสำเร็จ
    // เพื่อให้ Backend รู้จัก User คนนี้และบันทึกลงตาราง users
    [HttpPost("sync")]
    [Authorize]
    public async Task<IActionResult> SyncProfile([FromBody] UserProfileDto profile)
    {
        // แกะ User ID จาก Token
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        
        // เช็คว่ามี User ใน DB เราหรือยัง
        var existingUser = _context.Users.Find(userId);

        if (existingUser == null)
        {
            // ถ้ายังไม่มี ให้สร้างใหม่ (Register ลง DB เรา)
            var newUser = new User
            {
                UserId = userId,
                Email = profile.Email,
                FullName = profile.FullName,
                Phone = profile.Phone,
                IsAdmin = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(newUser);
        }
        else
        {
            // ถ้ามีแล้ว อัปเดตข้อมูลล่าสุด
            existingUser.FullName = profile.FullName;
            existingUser.Phone = profile.Phone;
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Profile Synced" });
    }
}