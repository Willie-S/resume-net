using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResuMeAPI.Data;
using ResuMeAPI.Interfaces;
using ResuMeAPI.Models;

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

        // GET: api/Profile
        [HttpGet]
        [AllowAnonymous]
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

                existingProfile.FirstName = profile.FirstName ?? existingProfile.FirstName;
                existingProfile.LastName = profile.LastName ?? existingProfile.LastName;
                existingProfile.Email = profile.Email ?? existingProfile.Email;
                existingProfile.Occupation = profile.Occupation ?? existingProfile.Occupation;

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

        private bool ProfileExists(int id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }
    }
}
