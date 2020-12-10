using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Authorization;
using WebApplication.BusinessLogic;
using WebApplication.Models;

namespace WebApplication.Controllers.RestApi
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "MarinaOwner,Manager,Admin")]
    public class SpotController : ControllerBase
    {
        private readonly ISpotService _spotService;
        private readonly IMarinaService _marinaService;
        private readonly UserService _userService;
        private readonly IAuthorizationService _authorizationService;

        public SpotController(ISpotService spotService, UserService userService,
            IMarinaService marinaService,
            IAuthorizationService authorizationService)
        {
            _spotService = spotService;
            _marinaService = marinaService;
            _userService = userService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Spot>>> GetSpots()
        {
            var user = await _userService.GetUserAsync(User);

            if (await _userService.IsInRoleAsync(user, RoleName.Manager))
            {
                return Ok(await _spotService.GetAll());
            }
            else if (await _userService.IsInRoleAsync(user, RoleName.MarinaOwner))
            {
                return Ok(await _spotService.GetAll(_userService.GetMarinaOwnerFromPerson(user).MarinaOwnerId));
            }
            return StatusCode(403);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Spot>> GetSpot(int id)
        {
            var spot = await _spotService.GetSingle(id);
            var isAuthorized = await _authorizationService.AuthorizeAsync(User, spot, Operation.Read);
            if (isAuthorized.Succeeded)
            {
                return Ok(spot);
            }

            return StatusCode(403);
        }

        [HttpPut]
        public async Task<IActionResult> PutSpot(int id, Spot spot)
        {
            if (id != spot.SpotId)
            {
                return BadRequest();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, spot, Operation.Update);

            if (isAuthorized.Succeeded)
            {
                _spotService.Update(spot);
                try
                {
                    await _spotService.Save();
                }
                catch (BusinessException ex)
                {
                    BadRequest(ex);
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Spot>> PostSpot(Spot spot)
        {
            if (spot.SpotId != 0 || spot.Location?.LocationId != 0)
            {
                BadRequest("Do not set the ID");
            }

            if (spot.MarinaId is not null)
            {
                var marina = await _marinaService.GetSingle(spot.MarinaId);
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Update);
                if (!isAuthorized.Succeeded)
                {
                    return StatusCode(403);
                }
            }
            await _spotService.Create(spot);
            await _spotService.Save();

            return CreatedAtAction("GetSpot", new { id = spot.SpotId }, spot);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpot(int id)
        {
            var spot = _spotService.GetSingle(id);

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, await spot, Operation.Delete);

            if (isAuthorized.Succeeded)
            {
                await _spotService.Delete(id);
                await _spotService.Save();
                return NoContent();
            }

            return StatusCode(403);
        }
    }
}
