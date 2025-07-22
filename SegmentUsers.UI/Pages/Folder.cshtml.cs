using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.Extensions;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Helpers;

namespace SegmentUsers.UI.Pages;

public class FolderModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ApiSettings apiSettings;

    public FolderModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        this.httpClientFactory = httpClientFactory;
            apiSettings = options.Value;
    }

    public Guid FolderId { get; set; }
    public string FolderName { get; set; }
    public List<NoteResponseDto>? Notes { get; set; } = new();
    public string Sort { get; set; } = "UpdatedAt";
    public bool Descending { get; set; }

    public async Task<IActionResult> OnGetAsync(Guid folderId, string sort = "UpdatedAt", bool descending = false)
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client is null)
            return RedirectToPage("/Login");

        FolderId = folderId;
        Sort = sort;
        Descending = descending;

        var response = await client.GetAsync($"/api/notes/folder/{folderId}?sortedBy={sort}&descending={descending}");
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return RedirectToPage("/Login");
        
        if (!response.IsSuccessStatusCode)
            return RedirectToPage("/Error");

        // var folderResponse = await client.GetAsync($"/api/folders/{folderId}");
        // if (folderResponse.IsSuccessStatusCode)
        // {
        //     var folderContent = await folderResponse.Content.ReadAsStringAsync();
        //     var folder = JsonSerializer.Deserialize<FolderDto>(folderContent,
        //         new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        //     FolderName = folder?.Name ?? "";
        // }
        //
        // if (folderResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        //     return RedirectToPage("/Login");

        var content = await response.Content.ReadAsStringAsync();
        Notes = JsonSerializer.Deserialize<List<NoteResponseDto>>(content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        FolderName = Notes?.FirstOrDefault()?.FolderName ?? "";

        return Page();
    }
}