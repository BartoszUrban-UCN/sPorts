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

            [Display(Name = "Manager")]
            public bool IsManager { get; set; }
        }

        private async Task LoadAsync(Person user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var isMarinaOwner = await _userManager.IsInRoleAsync(user, "MarinaOwner");
            var isBoatOwner = await _userManager.IsInRoleAsync(user, "BoatOwner");
            var isManager = await _userManager.IsInRoleAsync(user, "Manager");

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                IsBoatOwner = isBoatOwner,
                IsMarinaOwner = isMarinaOwner,
                IsManager = isManager
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
                    // First add it to the context as a boat owner, and then assign the role to that as well
                    // Take note of the order of operations, since only the AddToRoleAsync persists changes in the DB
                    await _userManager.AddToRoleAsync(user, "BoatOwner");
                    await _loginService.MakePersonBoatOwner(user);
                }
                // Else if he was one but decides not to be one anymore
                else
                {
                    // First revoke the boat owner rights, and then remove its role also
                    // Take note of the order of operations, since only the RemoveFromRoleAsync persists changes in the DB
                    await _userManager.RemoveFromRoleAsync(user, "BoatOwner");
                    await _loginService.RevokeBoatOwnerRights(user);
                }

                // Persist the changes to the database
                await _loginService.Save();
            }

            var isMarinaOwner = await _userManager.IsInRoleAsync(user, "MarinaOwner");
            // If user changed the marina owner status
            if (Input.IsMarinaOwner != isMarinaOwner)
            {
                // If he decided to become one
                if (Input.IsMarinaOwner)
                {
                    // First add it to the context as a marina owner, and then assign the role to that as well
                    // Take note of the order of operations, since only the AddToRoleAsync persists changes in the DB
                    await _loginService.MakePersonMarinaOwner(user);
                    await _userManager.AddToRoleAsync(user, "MarinaOwner");
                }
                // Else if he was one but decides not to be one anymore
                else
                {
                    // First revoke the marina owner rights, and then remove its role also
                    // Take note of the order of operations, since only the RemoveFromRoleAsync persists changes in the DB
                    await _loginService.RevokeMarinaOwnerRights(user);
                    await _userManager.RemoveFromRoleAsync(user, "MarinaOwner");
                }
            }

            var isManager = await _userManager.IsInRoleAsync(user, "Manager");
            // If user changed the admin status
            if (Input.IsManager != isManager)
            {
                // If he decided to become one
                if (Input.IsManager)
                    await _userManager.AddToRoleAsync(user, "Manager");
                // Else if he was one but decides not to be one anymore
                else
                    await _userManager.RemoveFromRoleAsync(user, "Manager");
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
