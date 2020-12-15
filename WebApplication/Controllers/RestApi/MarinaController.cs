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
    public class MarinaController : ControllerBase
    {
        private readonly ISpotService _spotService;
        private readonly IMarinaService _marinaService;
        private readonly UserService _userService;
        private readonly IAuthorizationService _authorizationService;

        public MarinaController(ISpotService spotService, UserService userService,
            IMarinaService marinaService,
            IAuthorizationService authorizationService)
        {
            _spotService = spotService;
            _marinaService = marinaService;
            _userService = userService;
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Gets all marinas with associations
        /// </summary>
        /// <returns>A list of marinas that the user is authorized to see</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marina>>> GetMarinas()
        {
            var user = await _userService.GetUserAsync(User);

            if (await _userService.IsInRoleAsync(user, RoleName.Manager))
            {
                return Ok(await _marinaService.GetAll());
            }
            else if (await _userService.IsInRoleAsync(user, RoleName.MarinaOwner))
            {
                return Ok(await _marinaService.GetAll(_userService.GetMarinaOwnerFromPerson(user).MarinaOwnerId));
            }
            return StatusCode(403);
        }

        /// <summary>
        /// Gets a single marina by Id with associations
        /// </summary>
        /// <param name="id">The id of the marina</param>
        /// <returns>
        /// A marina if it exists and the user is authorized to see it
        /// </returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Marina>> GetMarina(int id)
        {
            var spot = await _marinaService.GetSingle(id);
            if (spot is not null)
            {
                var isAuthorized = await _authorizationService.AuthorizeAsync(User, spot, Operation.Read);
                if (isAuthorized.Succeeded)
                {
                    return Ok(spot);
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Updates the marina under the given Id
        /// </summary>
        /// <param name="id">The id of the marina</param>
        /// <param name="marina">Updated marina information</param>
        /// <returns>Status code 204 if the update succeeded</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMarina(int id, Marina marina)
        {
            if (id != marina.MarinaId)
            {
                return BadRequest();
            }

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, marina, Operation.Update);

            if (isAuthorized.Succeeded)
            {
                _marinaService.Update(marina);
                try
                {
                    await _marinaService.Save();
                }
                catch (BusinessException ex)
                {
                    BadRequest(ex);
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a marina
        /// </summary>
        /// <param name="marina">The marina schema to create</param>
        /// <returns>The created marina with the id</returns>
        [HttpPost]
        public async Task<ActionResult<Marina>> PostSpot(Marina marina)
        {
            if (marina.MarinaId != 0 || marina.Location?.LocationId != 0)
            {
                BadRequest("Do not set the ID");
            }

            await _marinaService.Create(marina);
            await _marinaService.Save();

            return CreatedAtAction("GetSpot", new { id = marina.MarinaId }, marina);
        }

        /// <summary>
        /// Deletes the marina under the given id
        /// </summary>
        /// <param name="id">The id of the marina</param>
        /// <returns>204 if the delete succeeded</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpot(int id)
        {
            var marina = _marinaService.GetSingle(id);

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, await marina, Operation.Delete);

            if (isAuthorized.Succeeded)
            {
                await _marinaService.Delete(id);
                await _marinaService.Save();
                return NoContent();
            }

            return StatusCode(403);
        }
    }
}
