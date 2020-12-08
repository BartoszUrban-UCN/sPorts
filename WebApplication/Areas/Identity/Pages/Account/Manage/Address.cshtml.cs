using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Areas.Identity.Pages.Account.Manage
{
    public class AddressModel : PageModel
    {
        private readonly SignInManager<Person> _signInManager;
        private readonly UserService _userService;

        public AddressModel(
            SignInManager<Person> signInManager,
            UserService userService)
        {
            _signInManager = signInManager;
            _userService = userService;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userService.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userService.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userService.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userService.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            await _userService.UpdateAddress(user, new Address { City = Input.City, Country = Input.Country, PostalCode = Input.PostalCode, State = Input.State, Street = Input.Street });

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        private async Task LoadAsync(Person user)
        {
            var address = await _userService.GetAddressFromPerson(user);

            Input = new InputModel
            {
                City = address?.City,
                Country = address?.Country,
                PostalCode = address?.PostalCode,
                State = address?.State,
                Street = address?.Street
            };
        }

        public class InputModel
        {
            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "Country")]
            public string Country { get; set; }

            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

            [Display(Name = "State")]
            public string State { get; set; }

            [Display(Name = "Street")]
            public string Street { get; set; }
        }
    }
}
