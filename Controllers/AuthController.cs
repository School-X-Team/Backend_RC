using System.Security.Claims;
using System.Text;
using Backend_RC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Backend_RC.Services;
using StackExchange.Redis;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

namespace Backend_RC.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;
    private readonly IDatabase _redisDatabase;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration config, IEmailService emailService, IConnectionMultiplexer redis)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _emailService = emailService;
        _redisDatabase = redis.GetDatabase();
    }

    [HttpPost("register-step1")]
    public async Task<IActionResult> RegisterStep1([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
            return BadRequest("Пользователь с таким email уже существует.");

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            CreatedAt = DateTime.UtcNow
        };
        //криптографически стойкая генерация кода
        var verificationCodeBytes = new byte[4];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(verificationCodeBytes);
        }
        var verificationCode = (BitConverter.ToUInt32(verificationCodeBytes, 0) % 1000000).ToString("D6");

        //хранение кода в виде hashset с метаинформацией
        await _redisDatabase.HashSetAsync($"verification_code_{model.Email}", new HashEntry[]
            {
                new HashEntry("code",verificationCode),
                new HashEntry("attempts", 3) //ограничение числа попыток 
            });

        await _emailService.SendEmailAsync(user.Email, "Ваш код подтверждения", $"Ваш код подтверждения: {verificationCode}");

        return Ok(new
        {
            message = "Код отправлен на вашу почту. Пожалуйста, введите его, чтобы продолжить."
        });

    }

    [HttpPost("register-step2")]
    public async Task<IActionResult> RegisterStep2([FromBody] RegisterStep2Model model)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        //защита от брутфорса
        var key = $"verification_code_{model.Email}";
        var attempts = await _redisDatabase.HashGetAsync(key, "attempts");
        if (attempts.IsNullOrEmpty || int.Parse(attempts) <= 0)
        {
            return BadRequest("Превышено количество попыток ввода кода.");
        }

        var storedCode = await _redisDatabase.HashGetAsync(key, "code");
        if (storedCode.IsNullOrEmpty || storedCode != model.VerificationCode)
        {
            await _redisDatabase.HashDecrementAsync(key, "attempts", 1);
            return BadRequest("Неверный код подтверждения.");
        }

        await _redisDatabase.KeyDeleteAsync(key);

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            CreatedAt = DateTime.UtcNow
        };
        //генерация криптографически стойкого пароля
        byte[] passwordBytes = new byte[12];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(passwordBytes);
        }
        var password = Convert.ToBase64String(passwordBytes);
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new { message = "User registration failed", errors });
        }

        await _emailService.SendEmailAsync(user.Email, "Ваш пароль", $"Ваш пароль: {password} ");

        return Ok(new
        {
            message = "Пользователь зарегистрирован, пароль отправлен на почту"
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return Unauthorized("Непарвильный email или пароль");

        if (await _userManager.IsLockedOutAsync(user))
                    return Unauthorized("Ваш аккаунт временно заблокирован.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return Unauthorized("Неправильный email или пароль");

        

        var token = GenerateJwtToken(user);
        return Ok(new
        {
            token
        });
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users
            .Select(u => new
        {
            u.Id,
            u.Email,
            u.FirstName,
            u.LastName,
            u.CreatedAt
        }).ToListAsync();

        return Ok(users);
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "User") //добавление роли
        };

        var token = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(3),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
