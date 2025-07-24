using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Helpers;
using SegmentUsers.UI.Extensions;

namespace SegmentUsers.UI.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ApiSettings apiSettings;

    public IndexModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        this.httpClientFactory = httpClientFactory;
        apiSettings = options.Value;
    }

    [BindProperty(SupportsGet = true)]
    public string ViewMode { get; set; } = "Users"; // По умолчанию показываем пользователей

    public List<VkUserResponse>? Users { get; set; }
    public List<SegmentResponseDto>? Segments { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client is null)
            return RedirectToPage("/Login");

        if (ViewMode == "Users")
        {
            var response = await client.GetAsync("/api/users");
            if (!response.IsSuccessStatusCode)
                return RedirectToPage("/Login");

            var content = await response.Content.ReadAsStringAsync();
            Users = JsonSerializer.Deserialize<List<VkUserResponse>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        else if (ViewMode == "Segments")
        {
            var response = await client.GetAsync("/api/segments");
            if (!response.IsSuccessStatusCode)
                return RedirectToPage("/Login");

            var content = await response.Content.ReadAsStringAsync();
            Segments = JsonSerializer.Deserialize<List<SegmentResponseDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        return Page();
    }
}
