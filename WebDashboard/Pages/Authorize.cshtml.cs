using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebDashboard.Pages
{
    public class AuthorizeModel : PageModel
    {
        readonly IConfiguration _configuration;

        public AuthorizeModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost(string accessKey)
        {
            if (accessKey == (_configuration["AccessKey"]?.ToString() ??  Environment.GetEnvironmentVariable("AccessKey") ?? "123456"))
            {
                HttpContext.Session.SetInt32("IsAuthorized", 1);
                return Redirect("/");
            }
            return Page();
        }
    }
}
