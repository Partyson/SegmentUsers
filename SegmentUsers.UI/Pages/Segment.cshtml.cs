using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Extensions;
using SegmentUsers.UI.Helpers;

namespace SegmentUsers.UI.Pages;

public class SegmentModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ApiSettings apiSettings;

    public SegmentModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        this.httpClientFactory = httpClientFactory;
        this.apiSettings = options.Value;
    }

    public SegmentResponseDto? Segment { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid SegmentId { get; set; }

    [BindProperty]
    public int PercentToAssign { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid segmentId)
    {
        SegmentId = segmentId;

        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client == null)
            return RedirectToPage("/Login");

        var response = await client.GetAsync($"/api/segments/{segmentId}");
        if (!response.IsSuccessStatusCode)
            return NotFound();

        var content = await response.Content.ReadAsStringAsync();
        Segment = JsonSerializer.Deserialize<SegmentResponseDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return Page();
    }

    public async Task<IActionResult> OnPostAssignPercentAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client == null)
            return RedirectToPage("/Login");

        // Отправляем просто число процентов в теле запроса
        var response = await client.PostAsJsonAsync($"/api/segments/random-users/{SegmentId}", PercentToAssign);
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Ошибка при назначении пользователей.");
            // Обновим данные сегмента, чтобы заново отобразить страницу с ошибкой
            await OnGetAsync(SegmentId);
            return Page();
        }

        return RedirectToPage(new { segmentId = SegmentId });
    }
}
