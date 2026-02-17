using Supabase;
using Postgrest.Attributes;
using Postgrest.Models;
using dotenv.net;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
// --- 1. การจัดการ Configuration ---
var supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL") 
                  ?? builder.Configuration["Supabase:Url"] ?? "";
var supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_KEY") 
                  ?? builder.Configuration["Supabase:Key"] ?? "";

builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = supabaseUrl + "/auth/v1";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = "authenticated",
            ValidateIssuer = true,
            ValidIssuer = supabaseUrl + "/auth/v1"
        };
    });

builder.Services.AddAuthorization();

if (string.IsNullOrEmpty(supabaseUrl) || string.IsNullOrEmpty(supabaseKey))
{
    Console.WriteLine("Error: Missing Supabase URL or Key. Please check your .env file.");
}

// --- 2. การลงทะเบียน Services ---
builder.Services.AddScoped<Supabase.Client>(_ => 
    new Supabase.Client(supabaseUrl, supabaseKey, new SupabaseOptions {
        AutoConnectRealtime = true
    })
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 3. การตั้งค่า Middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Asset Management API V1");
        c.RoutePrefix = "swagger"; 
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
// --- 4. API Endpoints ---

app.MapGet("/", () => "Asset Management API is running!");

// GET Categories - เพิ่ม Task<IResult>
app.MapGet("/categories", async Task<IResult> (Supabase.Client client) => 
{
    try 
    {
        var result = await client.From<AssetCategory>().Get();
        var data = result.Models.Select(x => new AssetCategoryDto {
            CategoryId = x.CategoryId,
            TypeName = x.TypeName,
            Description = x.Description,
            CreatedAt = x.CreatedAt
        }).ToList();

        return Results.Ok(data);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database Error: {ex.Message}");
    }
});

// Auth: Register - ตรวจสอบ Task<IResult> เรียบร้อย
app.MapPost("/auth/register", async Task<IResult> (Supabase.Client client, AuthRequest request) => {
    try {
        var session = await client.Auth.SignUp(request.Email, request.Password);
        
        if (session?.User != null) {
            return Results.Ok(new { Message = "Registration successful", UserId = session.User.Id });
        }
        return Results.BadRequest("Registration failed: Could not create user session.");
    }
    catch (Exception ex) {
        return Results.BadRequest(new { Error = ex.Message });
    }
});

// Auth: Login - ตรวจสอบ Task<IResult> เรียบร้อย
app.MapPost("/auth/login", async Task<IResult> (Supabase.Client client, AuthRequest request) => {
    try {
        var session = await client.Auth.SignIn(request.Email, request.Password);
        
        if (session != null) {
            return Results.Ok(new { 
                Token = session.AccessToken, 
                RefreshToken = session.RefreshToken,
                ExpiresIn = session.ExpiresIn 
            });
        }
        return Results.Unauthorized();
    }
    catch (Exception ex) {
        return Results.Unauthorized();
    }
});
app.MapPost("/assets", async Task<IResult> (Supabase.Client client, AssetDto assetDto) => {
    try {
        var newAsset = new Asset {
            AssetId = Guid.NewGuid(),
            AssetName = assetDto.AssetName,
            AssetNumber = assetDto.AssetNumber,
            CategoryId = assetDto.CategoryId,
            Brand = assetDto.Brand,
            Model = assetDto.Model,
            SerialNumber = assetDto.SerialNumber,
            LocationId = assetDto.LocationId,
            Status = assetDto.Status ?? "Available"
        };

        var result = await client.From<Asset>().Insert(newAsset);
        var createdAsset = result.Models.FirstOrDefault();

        if (createdAsset != null) {
            // --- แก้ไขตรงนี้: สร้าง Object ใหม่เพื่อส่งกลับหน้าบ้าน (เลี่ยง BaseModel) ---
            var responseData = new {
                AssetId = createdAsset.AssetId,
                AssetName = createdAsset.AssetName,
                AssetNumber = createdAsset.AssetNumber,
                Status = createdAsset.Status,
                CreatedAt = createdAsset.CreatedAt
            };

            return Results.Created($"/assets/{createdAsset.AssetId}", responseData);
        }
        
        return Results.BadRequest("Failed to create asset.");
    }
    catch (Exception ex) {
        // ดักจับกรณีเลขรหัสซ้ำ (Unique Constraint)
        if (ex.Message.Contains("23505")) {
            return Results.Conflict(new { Message = "รหัสทรัพย์สิน (Asset Number) นี้มีอยู่ในระบบแล้ว" });
        }
        return Results.Problem($"Persistence Error: {ex.Message}");
    }
}).RequireAuthorization(); // ล็อกไว้ ต้องมี Token ถึงจะเข้าได้
app.Run();

// --- 5. Data Models ---

[Table("asset_categories")]
public class AssetCategory : BaseModel {
    [PrimaryKey("category_id", false)] public Guid CategoryId { get; set; }
    [Column("type_name")] public string? TypeName { get; set; }
    [Column("description")] public string? Description { get; set; }
    [Column("created_at")] public DateTime CreatedAt { get; set; }
}

public class AssetCategoryDto {
    public Guid CategoryId { get; set; }
    public string? TypeName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AuthRequest {
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

[Table("assets")]
public class Asset : BaseModel {
    [PrimaryKey("asset_id", false)] 
    public Guid AssetId { get; set; }

    [Column("asset_name")] 
    public string? AssetName { get; set; }

    [Column("asset_number")] 
    public string? AssetNumber { get; set; }

    [Column("category_id")] 
    public Guid? CategoryId { get; set; } // เป็น UUID และเป็น Optional

    [Column("brand")] 
    public string? Brand { get; set; }

    [Column("model")] 
    public string? Model { get; set; }

    [Column("serial_number")] 
    public string? SerialNumber { get; set; }

    [Column("location_id")] 
    public Guid? LocationId { get; set; }

    [Column("status")] 
    public string? Status { get; set; }

    [Column("created_at")] 
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")] 
    public DateTime UpdatedAt { get; set; }
}
public class AssetDto {
    public string AssetName { get; set; } = ""; // Required
    public string? AssetNumber { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? SerialNumber { get; set; }
    public Guid? LocationId { get; set; }
    public string? Status { get; set; }
}