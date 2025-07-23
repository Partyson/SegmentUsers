using System.Text.Json;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Extensions;
using SegmentUsers.UI.Helpers;

namespace SegmentUsers.UI.Pages;

public class AddUsersToSegmentModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ApiSettings apiSettings;

    public AddUsersToSegmentModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        this.httpClientFactory = httpClientFactory;
        apiSettings = options.Value;
    }

    [BindProperty(SupportsGet = true)]
    public Guid SegmentId { get; set; }

    [BindProperty]
    public string SegmentName { get; set; } = string.Empty;

    [BindProperty]
    public List<UserSelectionDto> AvailableUsers { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client == null)
            return RedirectToPage("/Login");

        var segment = await client.GetFromJsonAsync<SegmentResponseDto>($"/api/segments/{SegmentId}");
        if (segment == null)
            return NotFound();

        SegmentName = segment.Name;

        var allUsers = await client.GetFromJsonAsync<List<VkUserItemDto>>("/api/users");
        if (allUsers == null)
            return NotFound();

        var assignedIds = segment.Users?.Select(u => u.Id).ToHashSet() ?? [];

        AvailableUsers = allUsers
            .Where(u => !assignedIds.Contains(u.Id))
            .Select(u => new UserSelectionDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                LastName = u.LastName
            })
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client == null)
            return RedirectToPage("/Login");

        var selectedIds = AvailableUsers
            .Where(u => u.IsSelected)
            .Select(u => u.Id)
            .ToList();

        if (!selectedIds.Any())
        {
            ModelState.AddModelError(string.Empty, "Выберите хотя бы одного пользователя.");
            return await OnGetAsync();
        }

        var response = await client.PostAsJsonAsync($"/api/segments/users/{SegmentId}", selectedIds);
        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Не удалось назначить пользователей.");
            return await OnGetAsync();
        }

        return RedirectToPage("/Segment", new { segmentId = SegmentId });
    }

    public class UserSelectionDto : VkUserItemDto
    {
        public bool IsSelected { get; set; }
    }
}
