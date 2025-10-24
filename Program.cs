using CustomerApplication.CustomerApplication.Application.Interfaces;
using CustomerApplication.CustomerApplication.Application.Services;
using CustomerApplication.CustomerApplication.Domain.Entities;
using CustomerApplication.CustomerApplication.Domain.Repositories;
using CustomerApplication.CustomerApplication.Infrastructure.Persistence.Repositories;
using CustomerApplication.Data;
using CustomerApplication.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;


var builder = WebApplication.CreateBuilder(args);

// 🔹 Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
           .EnableDetailedErrors()
           .EnableSensitiveDataLogging());

// 🔹 AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// 🔹 Register repositories
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ILookupRepository, LookupRepository>();

// 🔹 Register services
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ILookupService, LookupService>();
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtOptions>>().Value);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// 🔹 Identity helpers
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// 🔹 JWT configuration (if used)
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

// 🔹 Controllers and JSON
builder.Services.AddControllers()
    .AddNewtonsoftJson();

// 🔹 Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",     // Angular default port
                "http://localhost:51462"     // Custom Angular dev port
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// 🔹 Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Enable CORS before auth
app.UseCors("AllowAngularApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
