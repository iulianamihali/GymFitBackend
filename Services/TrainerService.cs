using GymFit.Data;
using GymFit.DTOs;
using GymFit.DTOs.Trainer;
using GymFit.Models;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Services
{
    public class TrainerService
    {

        private readonly GymFitContext _context;
        public TrainerService(GymFitContext context)
        {
            _context = context;
        }

        public async Task<List<MonthlyProgressDto>> MonthlyProgress(Guid TrainerId)
        {
            var SessionsPerMonth = await _context.TrainingSessions
               .Where(e => e.TrainerId == TrainerId && e.StartDateTime.Year == DateTime.Now.Year)
               .GroupBy(e => e.StartDateTime.Month)
               .Select(g => new { Month = g.Key, Count = g.Count() }).ToListAsync();

            var result = new List<MonthlyProgressDto>();
            for (int i = 1; i <= 12; i++)
            {
                var match = SessionsPerMonth.FirstOrDefault(x => x.Month == i);

                result.Add(new MonthlyProgressDto
                {
                    Month = new DateTime(2025, i, 1).ToString("MMM"),
                    Sessions = match != null ? match.Count : 0
                });
            }
            return result;
        }

        public async Task<NextTrainingSessionDto?> NextTrainingSession(Guid TrainerId)
        {
            
            var result = await _context.TrainingSessions
                .Include(e => e.Client)
                    .ThenInclude(e => e.User)
                .Where(e => e.TrainerId == TrainerId && e.StartDateTime >= DateTime.Now)
                .OrderBy(e => e.StartDateTime)
                .FirstOrDefaultAsync();

            if (result == null)
                return null;

            NextTrainingSessionDto model = new NextTrainingSessionDto
            {
                Title = result.Title,
                StartDateTime = result.StartDateTime,
                ClientName = result.Client.User.FirstName + " " + result.Client.User.LastName,
                DurationInMinutes = result.DurationInMinutes,
            }; 
            return model; 
           
                
        }

        public async Task<SummaryTrainerDto> SummaryTrainer(Guid trainerId)
        {
            var listEnrollment = await _context.ClientTrainerEnrollments
                .Where(e => e.TrainerId == trainerId && e.EndDate >= DateTime.Now)
                .ToListAsync();
            var listCoursesActive = await _context.Courses
                .Where(e => e.TrainerId == trainerId && e.Active)
                .ToListAsync();
            var listNextSessions = await _context.TrainingSessions
                .Where(e => e.TrainerId == trainerId && e.StartDateTime >= DateTime.Now)
                .ToListAsync();
            SummaryTrainerDto summary = new SummaryTrainerDto
            {
                TotalClientsEnrolled = listEnrollment.Count,
                TotalCoursesActive = listCoursesActive.Count,
                TotalNextTrainingSessions = listNextSessions.Count,
            };
            return summary;
                

        }

        public async Task<List<CoursesCreatedDto>> CoursesCreated(Guid trainerId)
        {
            var result = await _context.Courses
                .Include(e => e.ClientCourseEnrollments)
           
                .Where(e => e.TrainerId == trainerId)
                .OrderByDescending(e => e.Active)
                .Select(e => new CoursesCreatedDto
                
                {
                    Id = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Price = e.Price,
                    MaxParticipants = e.MaxParticipants,
                    TotalParticipants = e.ClientCourseEnrollments.Count,
                    Active = e.Active,

                }).ToListAsync();
            return result;
        }

        public async Task<bool> EditCourse(CoursesCreatedDto info)
        {
            var result = await _context.Courses
                .Where(e => e.Id == info.Id)
                .FirstOrDefaultAsync();
            if(result != null)
            {
                result.Title = info.Title;
                result.Description = info.Description;
                result.Price = info.Price;
                result.MaxParticipants = info.MaxParticipants;
                result.Active = info.Active; 
                _context.Courses.Update(result);
                _context.SaveChanges();
                return true;
            }
            return false;
            
        }

        public async Task<bool> AddCourse(CoursesCreatedDto coursedto, Guid trainerId)
        {
            Course course = new Course
            {
                Id = Guid.NewGuid(),
                Title = coursedto.Title,
                Description = coursedto.Description,
                Price = coursedto.Price,
                MaxParticipants = coursedto.MaxParticipants,
                Active = coursedto.Active,
                TrainerId = trainerId
            };

            _context.Courses.Add(course);
            var result = (await _context.SaveChangesAsync()) > 0;
            return result;
        }
    }
}
