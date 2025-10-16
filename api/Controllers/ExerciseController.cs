using api.Data;
using Microsoft.AspNetCore.Mvc;
using api.Mappers;
using api.Interfaces;
using api.Dtos.Exercise;

namespace api.Controllers
{
    
    
    [Route("api/exercise")]
    [ApiController]
    public class ExerciseController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IExerciseRepository _exerciseRepo;
        public ExerciseController(ApplicationDBContext context, IExerciseRepository exerciseaRepo)
        {
            _exerciseRepo = exerciseaRepo;
            _context = context;
        }
        [HttpGet]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetAll()
        {
            var exercises = await _exerciseRepo.GetAllAsync();
            var exerciseDto = exercises.Select(s => s.ToExerciseDto());
            return Ok(exercises);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var exercise = await _exerciseRepo.GetByIdAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }
            return Ok(exercise.ToExerciseDto());
        }
        [HttpPost]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Create([FromBody] CreateExerciseRequestDto exerciseDto)
        {

            var exerciseModel = exerciseDto.ToExerciseFromCreateDto();
            await _exerciseRepo.CreateAsync(exerciseModel);
            return CreatedAtAction(nameof(GetById), new { id = exerciseModel.Id }, exerciseModel.ToExerciseDto());
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "1,2")]//van blacklist resze ezert user is kell

        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateExerciseRequestDto updateDto)
        {
            var exerciseModel = await _exerciseRepo.UpdateAsync(id, updateDto);

            if (exerciseModel == null)
            {
                return NotFound();
            }

            
            return Ok(exerciseModel.ToExerciseDto());
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var exerciseModel = await _exerciseRepo.DeleteAsync(id);
            if (exerciseModel == null)
            {
                return NotFound();
            }
            

            return NoContent();
        }
       
    }
    
}