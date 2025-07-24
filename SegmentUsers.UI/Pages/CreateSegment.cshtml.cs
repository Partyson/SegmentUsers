using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Helpers;
using System.Text.Json;
using SegmentUsers.UI.Extensions;

namespace SegmentUsers.UI.Pages;

public class CreateSegmentModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ApiSettings apiSettings;

    public CreateSegmentModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        this.httpClientFactory = httpClientFactory;
        apiSettings = options.Value;
    }

    [BindProperty]
    public CreateSegmentDto Input { get; set; } = new();

    public List<VkUserResponse> AvailableUsers { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client == null)
            return RedirectToPage("/Login");

        var response = await client.GetAsync("/api/users");
        if (!response.IsSuccessStatusCode)
            return RedirectToPage("/Login");

        var content = await response.Content.ReadAsStringAsync();
        AvailableUsers = JsonSerializer.Deserialize<List<VkUserResponse>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<VkUserResponse>();

        return Page();
    }

    public string ApiErrorMessage { get; set; } = string.Empty;

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

        var response = await client.PostAsJsonAsync("/api/segments", Input);
        if (response.IsSuccessStatusCode)
        {
            TempData["SuccessMessage"] = "Сегмент успешно создан.";
            return RedirectToPage("/Index");
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        ApiErrorMessage = errorContent;

        await OnGetAsync();
        return Page();
    }

}
