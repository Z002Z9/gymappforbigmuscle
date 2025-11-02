using Microsoft.AspNetCore.Mvc;
using api.Interfaces;
using System.Security.Claims;
using gymappforbigmuscle.Interfaces;
using api.Data;
using api.Dtos.Dailydata;
using api.Mappers;
using api.Models;
using gymappforbigmuscle.Dtos.User;
using gymappforbigmuscle.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace api.Controllers
{
    [ApiController]
    [Route("api/[generateworkout]")]
    public class WorkoutController : ControllerBase
    {
        
        
    }
}