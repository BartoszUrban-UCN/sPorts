using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Areas.Identity.Pages.Account.Manage
{
    public class ApiKeyModel : PageModel
    {
        private readonly SignInManager<Person> _signInManager;
        private readonly UserService _userService;

        public ApiKeyModel(
            SignInManager<Person> signInManager,
            UserService userService)
        {
            _signInManager = signInManager;
            _userService = userService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public string Token { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userService.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userService.GetUserId(User)}'.");
            }

            Token = Request.Headers["cookie"];

            return Page();
        }
    }
}
