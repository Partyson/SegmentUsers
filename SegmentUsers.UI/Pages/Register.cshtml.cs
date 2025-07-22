using System.Net;
using System.Net.Http.Json;
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

        var response = await client.PostAsJsonAsync("/register", new
        {
            Email = Input.Email,
            Password = Input.Password
        });

        if (response.IsSuccessStatusCode)
        {
            return RedirectToPage("/Login");
        }

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetailsResponse>();
            if (problemDetails?.Errors != null)
            {
                Errors = problemDetails.Errors
                    .SelectMany(kvp => kvp.Value.Select(msg => $"{msg}"))
                    .ToList();
            }
            else
                Errors.Add("Ошибка валидации данных.");

            return Page();
        }

        Errors.Add("Ошибка регистрации.");
        return Page();
    }
}