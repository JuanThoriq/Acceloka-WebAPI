using Acceloka.Entities;
using Microsoft.EntityFrameworkCore;
using Acceloka.Services.Implementations;
using Acceloka.Services.Interfaces;
using Serilog;
using MediatR;
using FluentValidation.AspNetCore;
using FluentValidation;



var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------
// 1. Konfigurasi Serilog
// ----------------------------------------------------------------
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()  // Log level = Information
    .WriteTo.File(
        path: Path.Combine("logs", $"Log-{DateTime.Now:yyyyMMdd}.txt"),
        rollingInterval: RollingInterval.Infinite, // Atau RollingInterval.Day jika mau per hari
        fileSizeLimitBytes: null // Boleh diatur jika ingin limit
    )
    .CreateLogger();

// Gunakan Serilog sebagai logger di ASP.NET Core
builder.Host.UseSerilog(logger);

// ----------------------------------------------------------------
// 2. Konfigurasi Connection String
// ----------------------------------------------------------------
var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("AccelokaDB");
if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Connection string 'AccelokaDB' tidak ditemukan di appsettings.json");
}

// ----------------------------------------------------------------
// 3. Registrasi DbContext (menggunakan DbContext Pool untuk performa)
// ----------------------------------------------------------------
builder.Services.AddDbContextPool<AccelokaContext>(options =>
    options.UseSqlServer(connectionString));

// ----------------------------------------------------------------
// 4. Registrasi Service (DI)
// ----------------------------------------------------------------
// Service untuk booking tiket
//builder.Services.AddScoped<IBookTicketService, BookTicketService>();

//// Service untuk mendapatkan available ticket
//builder.Services.AddScoped<IAvailableTicketService, AvailableTicketService>();

//// Service untuk mendapatkan detail booked ticket
//builder.Services.AddScoped<IBookedTicketDetailService, BookedTicketDetailService>();

//// Service untuk revoke ticket
//builder.Services.AddScoped<IRevokeTicketService, RevokeTicketService>();

//// Service untuk edit booked ticket
//builder.Services.AddScoped<IEditBookedTicketService, EditBookedTicketService>();

//// Service untuk MediatR
builder.Services.AddMediatR(cfg => {
    // Di sini kita register assembly
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

//// Service untuk FluentValidation
// 1.Tambahkan MVC/Controllers
builder.Services.AddControllers();

// 2. Registrasi FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// 3. Daftarkan semua validator di assembly ini
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);


// ----------------------------------------------------------------
// 5. Registrasi Controller
// ----------------------------------------------------------------
builder.Services.AddControllers();


// ----------------------------------------------------------------
// 6. Build dan Konfigurasi Pipeline HTTP
// ----------------------------------------------------------------
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
