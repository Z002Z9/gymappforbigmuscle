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


//a swaggerekhez 
namespace api.Controllers
{
    [Route("api/dailydata")]
    [ApiController]
    public class DailydataController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IDailydataRepository _dailydataRepo;
        public DailydataController(ApplicationDBContext context, IDailydataRepository dailydataRepo)
        {
            _dailydataRepo = dailydataRepo;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var dailydatas = await _dailydataRepo.GetAllAsync();
            var dailydataDto = dailydatas.Select(s => s.ToDailydataDto());
            return Ok(dailydatas);
        }

        [HttpGet("{id}")]
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
        public async Task<IActionResult> Create([FromBody] CreateDailydataRequestTrueDto dailydataDto)
        {

            var dailydataModel = dailydataDto.ToDailydataFromCreateDto();
            await _dailydataRepo.CreateAsync(dailydataModel);
            return CreatedAtAction(nameof(GetById), new { id = dailydataModel.Id }, dailydataModel.ToDailydataDto());
        }

        [HttpPut]
        [Route("{id}")]

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
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var dailydataModel = await _dailydataRepo.DeleteAsync(id);
            if (dailydataModel == null)
            {
                return NotFound();
            }
            

            return NoContent();
        }
       
    }
}