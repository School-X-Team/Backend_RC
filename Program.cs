using System.Text;
using Backend_RC.Models;
using Backend_RC.Repositories;
using Backend_RC.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

//настройка redis
var redisConfiguration = builder.Configuration.GetSection("Redis");
var redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
{
    EndPoints = { $"{redisConfiguration["Host"]}:{redisConfiguration["Port"]}" },
    Password = redisConfiguration["Password"],
    AbortOnConnectFail = false //отключает исключение при неудачном поключении
});
builder.Services.AddSingleton<IConnectionMultiplexer>(redis); //регистраци€ redis как сервиса

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //издатель будет валидироватьс€ при валидации токена
            ValidateIssuer = true,
            //издатель
            ValidIssuer = jwtSettings["Issuer"],
            //ѕотребитель токена будет валидироватьс€
            ValidateAudience = true,
            //установка потребител€ токена
            ValidAudience = jwtSettings["Audience"],
            //будет ли валидироватьс€ врем€ существовани€
            ValidateLifetime = true,
            //установка ключа безопасности
            IssuerSigningKey = new SymmetricSecurityKey(key),
            //валидации€ ключа безопасности
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ICardNumberGenerator, CardNumberGenerator>();
//нужно ли все сервисы прописывать в program cs?
builder.Services.AddScoped<IVCardRepository, VCardRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.Urls.Add("http://+:8080");
//app.Urls.Add("https://+:8081");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
