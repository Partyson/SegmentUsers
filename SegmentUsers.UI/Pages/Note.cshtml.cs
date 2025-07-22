using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.Extensions;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Enums;
using SegmentUsers.UI.Helpers;

namespace SegmentUsers.UI.Pages;

public class NoteModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ApiSettings apiSettings;

    public NoteModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        this.httpClientFactory = httpClientFactory;
        apiSettings = options.Value;
    }

    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IsEditing { get; set; }

    [BindProperty]
    public NoteDto Note { get; set; } = new();

    public List<FolderResponseDto> Folders { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    
    public async Task<IActionResult> OnPostToggleFavoriteAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client is null)
            return RedirectToPage("/Login");
        
        var response = await client.PatchAsync($"/api/notes/{Note.Id}/favorite", null);
        if (!response.IsSuccessStatusCode)
        {
            Errors.Add("Не удалось изменить статус избранного.");
            return Page();
        }

        var noteResponse = await client.GetFromJsonAsync<NoteDto>($"/api/notes/{Note.Id}");
        if (noteResponse is null)
            return NotFound();

        Note = noteResponse;

        return Page();
    }


    public async Task<IActionResult> OnGetAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client is null)
            return RedirectToPage("/Login");
        
        var noteResponse = await client.GetFromJsonAsync<NoteDto>($"/api/notes/{Id}");
        if (noteResponse == null) 
            return NotFound();

        Note = noteResponse;

        var foldersResponse = await client.GetAsync("/api/folders");
        
        if (foldersResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return RedirectToPage("/Login");
        
        if (!foldersResponse.IsSuccessStatusCode)
            Errors.Add("Не удалось получить папки.");
        
        Folders = await foldersResponse.Content.ReadFromJsonAsync<List<FolderResponseDto>>() ?? new();
        
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client is null)
            return RedirectToPage("/Login");

        var updateDto = new UpdateNoteDto
        {
            Title = Note.Title ?? string.Empty,
            Content = Note.Content ?? string.Empty,
            Importance = Enum.TryParse<Importance>(Note.Importance, out var imp) ? imp : Importance.Common,
            FolderId = Guid.TryParse(Note.FolderName, out var folderId) ? folderId : null
        };

        var response = await client.PatchAsJsonAsync($"/api/notes/{Note.Id}", updateDto);

        if (response.IsSuccessStatusCode)
            return RedirectToPage("/Note", new { id = Note.Id });

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

            var noteResponse = await client.GetFromJsonAsync<NoteDto>($"/api/notes/{Note.Id}");
            if (noteResponse != null)
                Note = noteResponse;

            await LoadFoldersAsync(client);
            return Page();
        }

        Errors.Add("Ошибка при обновлении заметки.");
        await LoadFoldersAsync(client);
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
