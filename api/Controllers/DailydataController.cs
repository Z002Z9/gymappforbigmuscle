using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;
using api.Data;
using Microsoft.AspNetCore.Mvc; 
using api.Mappers;
using api.Dtos.Dailydata;
using Microsoft.EntityFrameworkCore;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using gymappforbigmuscle.Interfaces;

//a swaggerekhez 
namespace api.Controllers
{
    [Route("api/dailydata")]
    [ApiController]
    
    public class DailydataController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDailydataRepository _dailydataRepo;
        private readonly ApplicationDBContext _context2;

        private readonly IUserRepository _userRepository;
        public DailydataController(ApplicationDBContext context, IDailydataRepository dailydataRepo, ApplicationDBContext _context2, IUserRepository userRepository)
        {
            _dailydataRepo = dailydataRepo;
            _context = context;
            _context2 = _context2;
            _userRepository = userRepository;
        }
        

        

        [HttpGet]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> GetAll()
        {
            var dailydatas = await _dailydataRepo.GetAllAsync();
            var dailydataDto = dailydatas.Select(s => s.ToDailydataDto());
            return Ok(dailydatas);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var dailydata = await _dailydataRepo.GetByIdAsync(id);
            if (dailydata == null)
            {
                return NotFound();
            }
            return Ok(dailydata.ToDailydataDto());
        }
        [HttpPost]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Create([FromBody] CreateDailydataRequestTrueDto dailydataDto)
        {
             var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Unauthorized();

                if (!int.TryParse(userIdClaim, out var userId))
                    return BadRequest("Invalid user id claim.");

                var dailyModel = dailydataDto.ToDailydataFromCreateDto(); // or your mapper
                dailyModel.UserId = userId; // ensure FK points to existing user

                await _dailydataRepo.CreateAsync(dailyModel); // or _context.Add/SaveChanges
                return CreatedAtAction(nameof(GetById), new { id = dailyModel.Id }, dailyModel);
            
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "1,2")]

        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateDailydataRequestDto updateDto)
        {
            var dailydataModel = await _dailydataRepo.UpdateAsync(id, updateDto);

            if (dailydataModel == null)
            {
                return NotFound();
            }

            
            return Ok(dailydataModel.ToDailydataDto());
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var dailydataModel = await _dailydataRepo.DeleteAsync(id);
            if (dailydataModel == null)
            {
                return NotFound();
            }


            return NoContent();
        }
        //just added
        //[HttpGet("user/{userId}")]
        [HttpGet("user/me")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetByUserId()
        {
            var userId2 = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            if (userId2 == null)
                return Unauthorized("You must be logged in");


            var user = await _userRepository.GetByIdAsync(userId2);

            if (user == null) return NotFound("User not found");

            var items = await _context.Dailydatas
                .Where(d => d.UserId == userId2)
                .OrderByDescending(d => d.Date)
                .ToListAsync();

            var dtos = items.Select(d => d.ToDailydataDto());
            return Ok(dtos);
        }
       
    }
}