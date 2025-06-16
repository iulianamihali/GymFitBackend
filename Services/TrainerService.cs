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

        public async Task<List<CoursesCreatedDto>> CoursesActiveCreated(Guid trainerId)
        {
            var result = await _context.Courses
                .Include(e => e.ClientCourseEnrollments)
                .Where(e => e.TrainerId == trainerId && e.Active)
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

        public async Task<List<TrainerClientDto>> GetClients(Guid trainerId)
        {
            var result = await _context.ClientTrainerEnrollments
                .Include(e => e.Client)
                    .ThenInclude(e => e.User)
                        .ThenInclude(e => e.Subscriptions)
                .Where(e => e.TrainerId == trainerId && e.EndDate >= DateTime.Now)
                .Select(e => new TrainerClientDto
                {
                    Name = e.Client.User.FirstName + " " + e.Client.User.LastName,
                    SubscriptionType = e.Client.User.Subscriptions
                        .OrderByDescending(s => s.ExpirationDate)
                        .Select(s => s.Name)
                            .FirstOrDefault() ?? "None",
                    Email = e.Client.User.Email,
                    PhoneNumber = e.Client.User.PhoneNumber,
                })
                .ToListAsync();
            return result;

         }


        public async Task<TrainerProfileSettingsDto> ProfileTrainer(Guid trainerId)
        {
            var result = await _context.Trainers
                .Include(e => e.User)
                .Where(e => e.UserId == trainerId)
                .Select(e => new TrainerProfileSettingsDto
                {
                    Id = e.UserId,
                    FirstName = e.User.FirstName,
                    LastName = e.User.LastName,
                    Address = e.User.Address,
                    Email = e.User.Email,
                    PhoneNumber = e.User.PhoneNumber,
                    Specialization = e.Specialization,
                    YearsOfExperience = e.YearsOfExperience,
                    Certification = e.Certification,
                    PricePerHour = e.PricePerHour,
                    StartInterval = e.StartInterval,
                    EndInterval = e.EndInterval,
                }).FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> EditProfileTrainer(TrainerProfileSettingsDto model)
        {
            var result = await _context.Trainers
                .Include(e => e.User)
                .Where(e => e.UserId == model.Id)
                .FirstOrDefaultAsync();
            if (result != null)
            {
                result.UserId = model.Id;
                result.User.FirstName = model.FirstName;
                result.User.LastName = model.LastName;
                result.User.Address = model.Address;
                result.User.Email = model.Email;
                result.User.PhoneNumber = model.PhoneNumber;
                result.Specialization = model.Specialization;
                result.YearsOfExperience = model.YearsOfExperience;
                result.Certification = model.Certification;
                result.PricePerHour = model.PricePerHour;
                result.StartInterval = model.StartInterval;
                result.EndInterval = model.EndInterval;

                _context.Trainers.Update(result);
                var rez = (await _context.SaveChangesAsync()) > 0;
                return rez;
             };
            return false;

            

        }
}
}
