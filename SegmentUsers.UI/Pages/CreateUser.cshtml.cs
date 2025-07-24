using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Helpers;
using System.Text.Json;
using SegmentUsers.UI.Extensions;

namespace SegmentUsers.UI.Pages;

public class CreateUserModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ApiSettings apiSettings;

    public CreateUserModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        this.httpClientFactory = httpClientFactory;
        apiSettings = options.Value;
    }

    [BindProperty]
    public CreateVkUserDto Input { get; set; } = new();

    public List<SegmentResponseDto> AvailableSegments { get; set; } = new();

    // Свойство для отображения ошибки от API в виде JSON
    public string ApiErrorMessage { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client == null)
            return RedirectToPage("/Login");

        var response = await client.GetAsync("/api/segments");
        if (!response.IsSuccessStatusCode)
            return RedirectToPage("/Login");

        var content = await response.Content.ReadAsStringAsync();
        AvailableSegments = JsonSerializer.Deserialize<List<SegmentResponseDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<SegmentResponseDto>();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client == null)
            return RedirectToPage("/Login");

        var response = await client.PostAsJsonAsync("/api/users", Input);
        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Пользователь успешно создан.";
            return RedirectToPage("/Index");
        }

        ApiErrorMessage = await response.Content.ReadAsStringAsync();

        await OnGetAsync();
        return Page();
    }
}
