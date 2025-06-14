using GymFit.DTOs;
using GymFit.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace GymFit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ODataController
    {
        private readonly ClientService _clientService;
        public ClientController(ClientService clientService) {
            _clientService = clientService; 
        }


        [HttpGet("monthly/{clientId}")]
        public IActionResult GetMonthlyProgress(Guid clientId)
        {
            var result = _clientService.GetMonthlyProgress(clientId);
            return Ok(result);
        }


        [HttpGet("courses/{clientId}")]
        public IActionResult GetClientCourses(Guid clientId)
        {
            var courses = _clientService.GetActiveCourses(clientId);
            return Ok(courses);
        }

        [HttpGet("nextSession/{clientId}")]
        public async Task<IActionResult> GetNextSession(Guid clientId)
        {
            var nextSession = await _clientService.GetNextTrainingAsync(clientId);
            return Ok(nextSession);
        }

        [HttpGet("activeSubscription/{clientId}")]
        public async Task<IActionResult> GetActiveSubscription(Guid clientId)
        {
            var activeSubscription = await _clientService.GetActiveSubscriptionAsync(clientId);
            return Ok(activeSubscription);
        }

        [HttpGet("courseDetails/{courseId}")]
        public async Task<IActionResult> GetCourseDetails(Guid courseId)
        {
            var details = await _clientService.GetCourseDetailsAsync(courseId);
            return Ok(details);
        }

        [HttpPost("trainerIntervals")]
        public async Task<IActionResult> GetTrainerIntervals(TrainerIntervalsRequestDto model)
        {
            var result = await _clientService.GetAvailableTrainerIntervals(model.ClientId, model.SelectedDate);
            return Ok(result);
        }
        [HttpPost("addSession")]
        public async Task<IActionResult> AddSession(AddSessionRequestDto model)
        {
            var result = await _clientService.AddSession(model);
            return Ok(result); 
        }
    }
}
