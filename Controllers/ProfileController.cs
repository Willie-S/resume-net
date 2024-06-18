using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResuMeAPI.Data;
using ResuMeAPI.Interfaces;
using ResuMeAPI.Models;
using ResuMeAPI.Utilities;

namespace ResuMeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ResuMeApiDbContext _context;
        private readonly IDbTransactionService _dbTransactionService;

        public ProfileController(ResuMeApiDbContext context, IDbTransactionService dbTransactionService)
        {
            _context = context;
            _dbTransactionService = dbTransactionService;
        }
        
        // TEST GET: api/Profile/TestAnon
        [HttpGet("TestAnon")]
        [AllowAnonymous]
        public IActionResult TestAnon()
        {
            try
            {
                return Ok(new
                {
                    Id = -1,
                    Name = "Test Anonymous getter successful"
                });
            }
            catch (Exception ex)
            {
                return Problem(ExceptionHelper.AggregateExceptionMessages(ex), title: "Unexpected error occurred");
            }
        }

        // TEST GET: api/Profile/Test
        [HttpGet("Test")]
        public IActionResult Test()
        {
            try
            {
                return Ok(new
                {
                    Id = -2,
                    Name = "Test Anonymous getter successful"
                });
            }
            catch (Exception ex)
            {
                return Problem(ExceptionHelper.AggregateExceptionMessages(ex), title: "Unexpected error occurred");
            }
        }

        // GET: api/Profile
        [HttpGet]
        public async Task<IActionResult> GetProfiles()
        {
            return await _dbTransactionService.ExecuteInTransactionAsync(async () =>
            {
                return Ok(await _context.Profiles.ToListAsync());
            });
        }

        // GET: api/Profile/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            return await _dbTransactionService.ExecuteInTransactionAsync(async () =>
            {
                var profile = await _context.Profiles.FindAsync(id);

                if (profile == null)
                {
                    return NotFound();
                }

                return Ok(profile);
            });
        }

        // PUT: api/Profile/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfile(int id, UpdateProfileDto profile)
        {
            return await _dbTransactionService.ExecuteInTransactionAsync(async () =>
            {
                var existingProfile = await _context.Profiles.FindAsync(id);

                if (existingProfile == null)
                {
                    return NotFound();
                }

                // Update only the provided fields
                EntityHelper.UpdateEntityProperties(profile, existingProfile);

                await _context.SaveChangesAsync();

                return NoContent();
            });
        }


        // POST: api/Profile
        [HttpPost]
        public async Task<IActionResult> PostProfile(Profile profile)
        {
            return await _dbTransactionService.ExecuteInTransactionAsync(async () =>
            {
                _context.Profiles.Add(profile);
                await _context.SaveChangesAsync();

                return CreatedAtAction("PostProfile", new { id = profile.Id }, profile);
            });
        }

        // DELETE: api/Profile/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfile(int id)
        {
            return await _dbTransactionService.ExecuteInTransactionAsync(async () =>
            {
                var profile = await _context.Profiles.FindAsync(id);

                if (profile == null)
                {
                    return NotFound();
                }

                _context.Profiles.Remove(profile);
                await _context.SaveChangesAsync();

                return NoContent();
            });
        }
    }
}
