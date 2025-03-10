using System.Net;
using System.Reflection;
using System.Text;
using Backend_RC.Models;
using Backend_RC.Repositories;
using Backend_RC.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
builder.Services.AddSingleton<IConnectionMultiplexer>(redis); //регистрация redis как сервиса

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
            //издатель будет валидироваться при валидации токена
            ValidateIssuer = true,
            //издатель
            ValidIssuer = jwtSettings["Issuer"],
            //Потребитель токена будет валидироваться
            ValidateAudience = true,
            //установка потребителя токена
            ValidAudience = jwtSettings["Audience"],
            //будет ли валидироваться время существования
            ValidateLifetime = true,
            //установка ключа безопасности
            IssuerSigningKey = new SymmetricSecurityKey(key),
            //валидациия ключа безопасности
            ValidateIssuerSigningKey = true
        };
    });
builder.Services.AddAuthorization(options =>
{
    var defaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();
    options.DefaultPolicy = defaultPolicy;
});

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<ICardNumberGenerator, CardNumberGenerator>();
//нужно ли все сервисы прописывать в program cs?
builder.Services.AddScoped<IVCardRepository, VCardRepository>();
builder.Services.AddScoped<IVCardService, VCardService>();
builder.Services.AddScoped<IIndicatedCardRepository, IndicatedCardRepository>();
builder.Services.AddScoped<IIndicatedCardService, IndicatedCardService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
         {
               new OpenApiSecurityScheme
                 {
                     Reference = new OpenApiReference
                     {
                         Type = ReferenceType.SecurityScheme,
                         Id = "Bearer"
                     }
                 },
                 new string[] {}
         }
     });
    // Включаем XML-документацию (чтобы подтягивались <summary>)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    option.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();
app.Urls.Add("http://+:8080");
//app.Urls.Add("https://+:8081");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
