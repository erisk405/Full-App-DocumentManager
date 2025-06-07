using System.Text;
using HRM_API.Data;
using HRM_API.Model;
using HRM_API.Repository;
using HRM_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://document-manager-bay.vercel.app")// อนุญาตแค่ Angular dev
        .AllowAnyMethod()
        .AllowAnyHeader();

    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])
            )
        };
    });


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var dbProvider = builder.Configuration.GetValue<string>("DatabaseProvider");

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (dbProvider == "Postgres")
    {
        var connStr = builder.Configuration.GetConnectionString("PostgresConnection");
        options.UseNpgsql(connStr);
    }
    else if (dbProvider == "SqlServer")
    {
        var connStr = builder.Configuration.GetConnectionString("SqlServerConnection");
        options.UseSqlServer(connStr);
    }
    else
    {
        throw new Exception("Unknown database provider");
    }
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);

Console.WriteLine($"[INFO] Using DB Provider: {dbProvider}");


// Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<CloudinaryService>();

builder.Services.AddScoped<DocumentRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<PermissionRepository>();
builder.Services.AddScoped<ImageRepository>();




var app = builder.Build();

// Use CORS
app.UseCors("AllowAngularDev");
app.UseStaticFiles();  // เพิ่มเพื่อให้เข้าถึงไฟล์ static ได้
// ให้เข้าถึงโฟลเดอร์ wwwroot/uploads
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profiles")),
    RequestPath = "/uploads"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
