using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using Microsoft.AspNetCore.Mvc;
using api.Mappers;
using api.Dtos.Dailydata;
using Microsoft.EntityFrameworkCore;


//a swaggerekhez
namespace api.Controllers
{
    [Route("api/dailydata")]
    [ApiController]
    public class DailydataController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        public DailydataController(ApplicationDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dailydatas = await _context.Dailydatas.ToListAsync();
            var dailydataDto = dailydatas.Select(s => s.ToDailydataDto());
            return Ok(dailydatas);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var dailydata = await _context.Dailydatas.FindAsync(id);
            if (dailydata == null)
            {
                return NotFound();
            }
            return Ok(dailydata.ToDailydataDto());
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDailydataRequestTrueDto dailydataDto)
        {

            var dailydataModel = dailydataDto.ToDailydataFromCreateDto();
            await _context.Dailydatas.AddAsync(dailydataModel);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = dailydataModel.Id }, dailydataModel.ToDailydataDto());
        }

        [HttpPut]
        [Route("{id}")]

        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateDailydataRequestDto updateDto)
        {
            var dailydataModel = await _context.Dailydatas.FirstOrDefaultAsync(x => x.Id == id);

            if (dailydataModel == null)
            {
                return NotFound();
            }

            dailydataModel.Date = updateDto.Date;
            dailydataModel.Weight = updateDto.Weight;
            dailydataModel.Dailykcalintake = updateDto.Dailykcalintake;
            dailydataModel.Trainedtoday = updateDto.Trainedtoday;

            await _context.SaveChangesAsync();
            return Ok(dailydataModel.ToDailydataDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var dailydataModel = await _context.Dailydatas.FirstOrDefaultAsync(x => x.Id == id);
            if (dailydataModel == null)
            {
                return NotFound();
            }
            _context.Dailydatas.Remove(dailydataModel);
            await _context.SaveChangesAsync();
            return NoContent();
        }
       
    }
}