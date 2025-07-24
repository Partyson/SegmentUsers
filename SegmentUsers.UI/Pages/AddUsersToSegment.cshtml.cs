using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Extensions;
using SegmentUsers.UI.Helpers;

namespace SegmentUsers.UI.Pages
{
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

        public List<VkUserItemDto> AvailableUsers { get; set; } = new();

        [BindProperty]
        public List<Guid> SelectedUserIds { get; set; } = new();

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

            var assignedIds = segment.Users?.Select(u => u.Id).ToHashSet() ?? new HashSet<Guid>();

            AvailableUsers = allUsers
                .Where(u => !assignedIds.Contains(u.Id))
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
            if (client == null)
                return RedirectToPage("/Login");

            if (SelectedUserIds == null || !SelectedUserIds.Any())
            {
                ModelState.AddModelError(string.Empty, "Выберите хотя бы одного пользователя.");
                await LoadAvailableUsersAsync();
                return Page();
            }

            var response = await client.PostAsJsonAsync($"/api/segments/users/{SegmentId}", SelectedUserIds);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Не удалось назначить пользователей.");
                await LoadAvailableUsersAsync();
                return Page();
            }

            TempData["AssignSuccess"] = "Пользователи успешно назначены.";

            await LoadAvailableUsersAsync();
            return Page();
        }

        private async Task LoadAvailableUsersAsync()
        {
            var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
            var segment = await client.GetFromJsonAsync<SegmentResponseDto>($"/api/segments/{SegmentId}");
            if (segment == null) return;

            SegmentName = segment.Name;

            var allUsers = await client.GetFromJsonAsync<List<VkUserItemDto>>("/api/users");
            if (allUsers == null) return;

            var assignedIds = segment.Users?.Select(u => u.Id).ToHashSet() ?? new HashSet<Guid>();

            AvailableUsers = allUsers
                .Where(u => !assignedIds.Contains(u.Id))
                .ToList();
        }
    }
}
