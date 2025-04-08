using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AttendanceDatabase.Models
{
    public class IndexModel : PageModel
    {
        public const string SessionKeyName = "_Name";
        public const string SessionKeyRole = "_Role";

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
            {
                HttpContext.Session.SetString(SessionKeyName, "temp");
                HttpContext.Session.SetString(SessionKeyRole, SessionKeyRole);
            }
            var name = HttpContext.Session.GetString(SessionKeyName);
            var role = HttpContext.Session.GetString(SessionKeyRole);

            _logger.LogInformation("Session Name: {Name}", name);
            _logger.LogInformation("Session Role: {Role}", role);
        }
    }
}