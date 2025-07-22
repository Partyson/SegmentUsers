using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using SegmentUsers.UI.DTOs;
using SegmentUsers.UI.Helpers;

namespace SegmentUsers.UI.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ApiSettings apiSettings;

        public LoginModel(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiOptions)
        {
            this.httpClientFactory = httpClientFactory;
            apiSettings = apiOptions.Value;
        }

        [BindProperty]
        public LoginInput Input { get; set; }

        public string ErrorMessage { get; set; }

        public class LoginInput
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var httpClient = httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(apiSettings.BaseUrl);

            var response = await httpClient.PostAsJsonAsync("/login", Input);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (json != null && !string.IsNullOrEmpty(json.AccessToken))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, Input.Email),
                        new Claim("AccessToken", json.AccessToken),
                        new Claim("RefreshToken", json.RefreshToken)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                    return RedirectToPage("/Index");
                }
            }

            ErrorMessage = "Неверный логин или пароль";
            return Page();
        }
    }
}