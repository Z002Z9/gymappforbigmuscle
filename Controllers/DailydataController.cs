using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using Microsoft.AspNetCore.Mvc;
using api.Mappers;
using api.Dtos.Dailydata;


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
        public IActionResult GetAll()
        {
            var dailydatas = _context.Dailydatas.ToList()
                .Select(s => s.ToDailydataDto());
            return Ok(dailydatas);
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var dailydata = _context.Dailydatas.Find(id);
            if (dailydata == null)
            {
                return NotFound();
            }
            return Ok(dailydata.ToDailydataDto());
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateDailydataRequestTrueDto dailydataDto)
        {

            var dailydataModel = dailydataDto.ToDailydataFromCreateDto();
            _context.Dailydatas.Add(dailydataModel);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = dailydataModel.Id }, dailydataModel.ToDailydataDto());
        }    

        
       
    }
}