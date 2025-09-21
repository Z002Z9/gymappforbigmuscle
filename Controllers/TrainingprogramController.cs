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
using api.Dtos.Trainingprogram;

namespace api.Controllers
{
    [Route("api/trainingprogram")]
    [ApiController]
    public class TrainingprogramController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly ITrainingprogramRepository _trainingprogramRepo;
        public TrainingprogramController(ApplicationDBContext context, ITrainingprogramRepository trainingprogramRepo)
        {
            _trainingprogramRepo = trainingprogramRepo;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var trainingprograms = await _trainingprogramRepo.GetAllAsync();
            var trainingprogramDto = trainingprograms.Select(s => s.ToTrainingprogramDto());
            return Ok(trainingprograms);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var trainingprogram = await _trainingprogramRepo.GetByIdAsync(id);
            if (trainingprogram == null)
            {
                return NotFound();
            }
            return Ok(trainingprogram.ToTrainingprogramDto());
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTrainingprogramRequestDto trainingprogramDto)
        {

            var trainingprogramModel = trainingprogramDto.ToTrainingprogramFromCreateDto();
            await _trainingprogramRepo.CreateAsync(trainingprogramModel);
            return CreatedAtAction(nameof(GetById), new { id = trainingprogramModel.Id }, trainingprogramModel.ToTrainingprogramDto());
        }

        [HttpPut]
        [Route("{id}")]

        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTrainingprogramRequestDto updateDto)
        {
            var trainingprogramModel = await _trainingprogramRepo.UpdateAsync(id, updateDto);

            if (trainingprogramModel == null)
            {
                return NotFound();
            }

            
            return Ok(trainingprogramModel.ToTrainingprogramDto());
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var trainingprogramModel = await _trainingprogramRepo.DeleteAsync(id);
            if (trainingprogramModel == null)
            {
                return NotFound();
            }
            

            return NoContent();
        }
    }
}