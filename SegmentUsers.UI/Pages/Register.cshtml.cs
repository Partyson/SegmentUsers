using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Helpers;

namespace SegmentUsers.UI.Pages;

public class RegisterModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ApiSettings apiSettings;

    public RegisterModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        this.httpClientFactory = httpClientFactory;
        apiSettings = options.Value;
    }

    [BindProperty]
    public RegisterInput Input { get; set; }

    public List<string> Errors { get; set; } = new();

    public class RegisterInput
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Input.Password != Input.ConfirmPassword)
        {
            Errors.Add("Пароли не совпадают.");
            return Page();
        }

        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(apiSettings.BaseUrl);

        // 1. Регистрация
        var registerResponse = await client.PostAsJsonAsync("/register", new
        {
            Email = Input.Email,
            Password = Input.Password
        });

        if (!registerResponse.IsSuccessStatusCode)
        {
            if (registerResponse.StatusCode == HttpStatusCode.BadRequest)
            {
                var problemDetails = await registerResponse.Content.ReadFromJsonAsync<ValidationProblemDetailsResponse>();
                if (problemDetails?.Errors != null)
                {
                    Errors = problemDetails.Errors
                        .SelectMany(kvp => kvp.Value.Select(msg => msg))
                        .ToList();
                }
                else
                {
                    Errors.Add("Ошибка валидации данных.");
                }
            }
            else
            {
                Errors.Add("Ошибка регистрации.");
            }

            return Page();
        }

        // 2. Автоматический логин
        var loginResponse = await client.PostAsJsonAsync("/login", new
        {
            Email = Input.Email,
            Password = Input.Password
        });

        if (!loginResponse.IsSuccessStatusCode)
        {
            Errors.Add("Регистрация прошла успешно, но вход не выполнен.");
            return RedirectToPage("/Login");
        }

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        if (loginResult != null && !string.IsNullOrEmpty(loginResult.AccessToken))
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Input.Email),
                new Claim("AccessToken", loginResult.AccessToken),
                new Claim("RefreshToken", loginResult.RefreshToken)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToPage("/Index");
        }

        Errors.Add("Регистрация прошла, но не удалось войти.");
        return RedirectToPage("/Login");
    }
}
