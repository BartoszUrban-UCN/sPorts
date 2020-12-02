using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<Person> _userManager;
        private readonly SignInManager<Person> _signInManager;
        private readonly ILoginService _loginService;

        public IndexModel(
            UserManager<Person> userManager,
            SignInManager<Person> signInManager,
            ILoginService loginService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _loginService = loginService;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Marina Owner")]
            public bool IsMarinaOwner { get; set; }

            [Display(Name = "Boat Owner")]
            public bool IsBoatOwner { get; set; }

            [Display(Name = "Admin")]
            public bool IsAdmin { get; set; }
        }

        private async Task LoadAsync(Person user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var isMarinaOwner = await _userManager.IsInRoleAsync(user, "MarinaOwner");
            var isBoatOwner = await _userManager.IsInRoleAsync(user, "BoatOwner");
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                IsBoatOwner = isBoatOwner,
                IsMarinaOwner = isMarinaOwner,
                IsAdmin = isAdmin
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            var isBoatOwner = await _userManager.IsInRoleAsync(user, "BoatOwner");
            // If user changed the boat owner status
            if (Input.IsBoatOwner != isBoatOwner)
            {
                // If he decided to become one
                if (Input.IsBoatOwner)
                {
                    await _userManager.AddToRoleAsync(user, "BoatOwner");
                    await _loginService.MakePersonBoatOwner(user);
                }
                // Else if he was one but decides not to be one anymore
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, "BoatOwner");
                    await _loginService.RevokeBoatOwnerRights(user);
                }
            }

            var isMarinaOwner = await _userManager.IsInRoleAsync(user, "MarinaOwner");
            // If user changed the marina owner status
            if (Input.IsMarinaOwner != isMarinaOwner)
            {
                // If he decided to become one
                if (Input.IsMarinaOwner)
                {
                    await _userManager.AddToRoleAsync(user, "MarinaOwner");
                    await _loginService.MakePersonMarinaOwner(user);
                }
                // Else if he was one but decides not to be one anymore
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, "MarinaOwner");
                    await _loginService.RevokeMarinaOwnerRights(user);
                }
            }

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            // If user changed the admin status
            if (Input.IsAdmin != isAdmin)
            {
                // If he decided to become one
                if (Input.IsAdmin)
                    await _userManager.AddToRoleAsync(user, "Admin");
                // Else if he was one but decides not to be one anymore
                else
                    await _userManager.RemoveFromRoleAsync(user, "Admin");
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
