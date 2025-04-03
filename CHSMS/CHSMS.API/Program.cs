using CHSMS.API.Configuration;
using CHSMS.API.Middleware;
using CHSMS.API.Models;
using CHSMS.API.Repositories;
using CHSMS.API.Repositories.Interfaces;
using CHSMS.API.Services;
using CHSMS.API.Services.Interfaces;
using CHSMS.API.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NETCore.MailKit.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("SEP_DB");
builder.Services.AddDbContext<SEP_TestContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddMailKit(config => config.UseMailKit(
    new NETCore.MailKit.Infrastructure.Internal.MailKitOptions()
    {
        Server = builder.Configuration["Smtp:Server"],
        Port = int.Parse(builder.Configuration["Smtp:Port"]),
        SenderEmail = builder.Configuration["Smtp:SenderEmail"],
        SenderName = builder.Configuration["Smtp:SenderName"],
        Account = builder.Configuration["Smtp:Username"],
        Password = builder.Configuration["Smtp:Password"],
        Security = true
    }
));
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<MedicalRecordHistoryRepository>();
builder.Services.AddScoped<MedicalRecordRepository>();

builder.Services.AddScoped<MedicalRecordHistoryService>();
builder.Services.AddScoped<MedicalRecordService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<MedicineRepository>();
builder.Services.AddScoped<MedicineService>();
builder.Services.AddScoped<MedicineRepository>();
builder.Services.AddScoped<MedicineService>();
builder.Services.AddScoped<PrescriptionRepository>();
builder.Services.AddScoped<PrescriptionService>();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new string[] {}
                }
            });
});

var secretKey = builder.Configuration["Jwt:Key"];
var secretKeyByte = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(secretKeyByte),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseCors("AllowLocalhost");

app.UseCors("AllowAllOrigins");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TokenBlacklistMiddleware>();

app.MapControllers();

app.Run();
