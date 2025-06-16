using GymFit.DTOs;
using GymFit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace GymFit.Controllers
{
    [Authorize(Roles = "Client")]
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


        [EnableQuery]
        [HttpGet("trainerCardInfo")]
        public IQueryable<TrainerCardResponseDto> GetTrainerCardInfo()
        {
            return _clientService.GetTrainerCardInfoAsQueryable();
        }

        [HttpGet("trainers")]
        public IActionResult GetTrainerCardInfo([FromQuery] string? specialization)
        {
            var result = _clientService.GetTrainersAsQueryable(specialization);
            return Ok(result.ToList());
        }

        [HttpGet("allSpecializations")]
        public async Task<IActionResult> GetAllSpecializations()
        {
            var result = await _clientService.GetAllSpecializations();
            return Ok(result);
        }

        [HttpPost("addEnrollmentClientTrainer")]
        public async Task<IActionResult> AddEnrollment(EnrollmentClientTrainerModelDto model)
        {
            var result = await _clientService.AddEnrollment(model);
            return Ok(result);
        }

        [HttpGet("myTrainers/{clientId}")]
        public async Task<IActionResult> MyTrainers(Guid clientId)
        {
            var result = await _clientService.GetMyTrainers(clientId);
            return Ok(result);  
        }

        [EnableQuery]
        [HttpGet("coursesCardInfo")]
        public IActionResult CoursesCardInfo([FromQuery] string? searchCourse)
        {
            var result = _clientService.GetCourses(searchCourse);
            return Ok(result);
        }

        [HttpPost("enrollmentCourse")]
        public async Task<IActionResult> EnrollmentCourse(EnrollmentCourseDto model)
        {
            var result = await _clientService.EnrollmentCourse(model);
            return Ok(result);
        }

        [HttpGet("settingsInfoClient/{clientId}")]
        public async Task<IActionResult> SettingsInfoClient(Guid clientId)
        {
            var result = await _clientService.GetInfoUser(clientId);
            return Ok(result);
        }

        [HttpPut("editProfile")]
        public async Task<IActionResult> EditProfile(SettingsClientInfoDto model)
        {
            var result = await _clientService.EditProfile(model);
            return Ok(result);
        }

        [HttpPost("addReview")]
        public async Task<IActionResult> AddReview(LeaveReviewModelDto model)
        {
            var result = await _clientService.LeaveReview(model);
            return Ok(result);
        }

        [HttpGet("getReviews/{trainerId}")]
        public async Task<IActionResult> getReviews(Guid trainerId)
        {
            var result = await _clientService.GetReviews(trainerId);
            return Ok(result);
        }
    }
}
