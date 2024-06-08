using System.Text;
using LicensePlateDataShared.Models;
using LicensePlateServer.Configurations;
using LicensePlateServer.Data;
using LicensePlateServer.Factories;
using LicensePlateServer.Repositories;
using LicensePlateServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
var connString = builder.Configuration.GetConnectionString("LicensePlateDbConnection");
builder.Services.AddDbContext<LicensePlateDbContext>(options => options.UseNpgsql(connString));

builder.Services.AddHostedService<CameraBackgroundService>();

builder.Services.AddIdentityCore<ApiUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<LicensePlateDbContext>();

builder.Services.AddScoped < ILicensePlateRecognition, LicensePlateRecognition>();
builder.Services.AddTransient<Camera>();
builder.Services.AddScoped<ICameraService, CameraService>();
builder.Services.AddTransient<ICameraCaptureFactory, CameraCaptureFactory>();
builder.Services.AddTransient<ICameraCapture, CameraCapture>();
builder.Services.AddScoped<ILicensePlateRepository, LicensePlateRepository>();
builder.Services.AddScoped<ICameraRepository, CameraRepository>();

builder.Services.AddAutoMapper(typeof(MapperConfig));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog((ctx, lc) => 
    lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddCors( options => {
    options.AddPolicy("AllowAll", 
        b => b.AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin());
});

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
