using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.Extensions;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Helpers;

namespace SegmentUsers.UI.Pages;

public class AllNotesModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ApiSettings apiSettings;

    public AllNotesModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        this.httpClientFactory = httpClientFactory;
        apiSettings = options.Value;
    }

    public List<NoteResponseDto> Notes { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string SearchQuery { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client is null)
            return RedirectToPage("/Login");
        
        var queryParam = string.IsNullOrWhiteSpace(SearchQuery) ? "" : $"?query={Uri.EscapeDataString(SearchQuery)}";
        var response = await client.GetAsync($"/api/notes{queryParam}");
        
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return RedirectToPage("/Login");
        
        if(!response.IsSuccessStatusCode)
            return RedirectToPage("/Error");
            
        var notes = await response.Content.ReadFromJsonAsync<List<NoteResponseDto>>();
        Notes = notes ?? new List<NoteResponseDto>();
        

        return Page();
    }

    
}