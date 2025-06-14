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
    }
}
