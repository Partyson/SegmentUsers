using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.Extensions;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Helpers;

namespace SegmentUsers.UI.Pages;

public class CreateNoteModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ApiSettings apiSettings;

    public CreateNoteModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        this.httpClientFactory = httpClientFactory;
        apiSettings = options.Value;
    }

    [BindProperty]
    public NoteInput Input { get; set; } = new();
    public List<FolderResponseDto> Folders { get; set; } = new();
    
    [BindProperty]
    public NoteResponseDto NoteResponse { get; set; } = new();
    
    [BindProperty(SupportsGet = true)]
    public Guid? FolderId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string FolderName { get; set; }

    public List<string> Errors { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client is null)
            return RedirectToPage("/Login");

        if (FolderId.HasValue)
            NoteResponse.FolderName = FolderName;

        var response = await client.PostAsJsonAsync("/api/notes", Input);

        if (response.IsSuccessStatusCode)
            return RedirectToPage("/Index");

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return RedirectToPage("/Login");

        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
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
            
            await LoadFoldersAsync(client);
            return Page();
        }

        Errors.Add("Ошибка при создании заметки.");
        return Page();
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client is null)
            return RedirectToPage("/Login");

        await LoadFoldersAsync(client);
        
        if (client.DefaultRequestHeaders.Authorization == null)
            return RedirectToPage("/Login");

        return Page();
    }
    
    private async Task LoadFoldersAsync(HttpClient client)
    {
        var response = await client.GetAsync("/api/folders");
        if (response.IsSuccessStatusCode)
        {
            var folders = await response.Content.ReadFromJsonAsync<List<FolderResponseDto>>();
            Folders = folders ?? [];
        }
    }

}