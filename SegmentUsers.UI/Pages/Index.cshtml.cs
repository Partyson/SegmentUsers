using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.Extensions;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Helpers;

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

    public List<FolderResponseDto>? Folders { get; set; } = new();
    public List<NoteResponseDto>? Notes { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client is null)
            return RedirectToPage("/Login");
        
        var foldersResponse = await client.GetAsync("/api/folders");
        var notesResponse = await client.GetAsync("/api/notes/root");
        if (foldersResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized 
            || notesResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return RedirectToPage("/Login");

        var folderContent = await foldersResponse.Content.ReadAsStringAsync();
        Folders = JsonSerializer.Deserialize<List<FolderResponseDto>>(folderContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var notesContent = await notesResponse.Content.ReadAsStringAsync();
        Notes = JsonSerializer.Deserialize<List<NoteResponseDto>>(notesContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return Page();
    }
}



