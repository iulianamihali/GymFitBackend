using GymFit.DTOs.Trainer;
using GymFit.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace GymFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainerController : ODataController
    {
        private readonly TrainerService _trainerService;
        public TrainerController(TrainerService trainerService)
        {
            _trainerService = trainerService;
        }


        [HttpGet("monthlyProgress/{trainerId}")]
        public async Task<IActionResult> MonthlyProgress(Guid TrainerId)
        {
            var result = await _trainerService.MonthlyProgress(TrainerId);
            return Ok(result);
        }

        [HttpGet("nextSession/{trainerId}")]
        public async Task<IActionResult> NextTrainingSession(Guid TrainerId)
        {
            var result = await _trainerService.NextTrainingSession(TrainerId);
            return Ok(result);
        }

        [HttpGet("summaryTrainerActivity/{trainerId}")]
        public async Task<IActionResult> SummaryTrainerActivity(Guid TrainerId)
        {
            var result = await _trainerService.SummaryTrainer(TrainerId);
            return Ok(result);
        }

        [HttpGet("coursesCreatedByTrainer/{trainerId}")]
        public async Task<IActionResult> CoursesCreated(Guid trainerId)
        {
            var result = await _trainerService.CoursesCreated(trainerId);
            return Ok(result);
        }

        [HttpPut("editCourse")]
        public async Task<IActionResult> EditCourse(CoursesCreatedDto model)
        {
            var result = await _trainerService.EditCourse(model);
            return Ok(result);
        }

        [HttpPost("addCourse/{trainerId}")]
        public async Task<IActionResult> AddCourse([FromBody] CoursesCreatedDto coursedto, [FromRoute] Guid trainerId)
        {
            var result = await _trainerService.AddCourse(coursedto, trainerId);
            return Ok(result);
        }

        [HttpGet("allClients/{trainerId}")]
        public async Task<IActionResult> AllClients(Guid trainerId)
        {
            var result = await _trainerService.GetClients(trainerId);
            return Ok(result);
        }
    }
}
