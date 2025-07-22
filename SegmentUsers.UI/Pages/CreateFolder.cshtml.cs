using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.Extensions;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Helpers;

namespace SegmentUsers.UI.Pages;

public class CreateFolderModel : PageModel
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ApiSettings apiSettings;

    public CreateFolderModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options)
    {
        this.httpClientFactory = httpClientFactory;
        apiSettings = options.Value;
    }

    [BindProperty]
    public string FolderName { get; set; }

    [BindProperty]
    public IFormFile Image { get; set; }
    public List<string> Errors { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        var client = httpClientFactory.CreateAuthorizedHttpClient(HttpContext, apiSettings);
        if (client is null)
            return RedirectToPage("/Login");

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(FolderName ?? string.Empty), "Name");

        if (Image != null && Image.Length > 0)
        {
            using var ms = new MemoryStream();
            await Image.CopyToAsync(ms);
            ms.Seek(0, SeekOrigin.Begin);
            content.Add(new ByteArrayContent(ms.ToArray()), "Image", Image.FileName);
        }

        var response = await client.PostAsync($"/api/folders?Name={FolderName}", content);

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

            return Page();
        }

        Errors.Add("Ошибка при создании папки.");
        return Page();
    }
}