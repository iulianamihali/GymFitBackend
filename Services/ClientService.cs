using GymFit.Data;
using GymFit.DTOs;
using GymFit.Models;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace GymFit.Services
{
    public class ClientService
    {
        private readonly GymFitContext _context;
        public ClientService(GymFitContext context)
        {
            _context = context;
        }

        public List<MonthlyProgressDto> GetMonthlyProgress(Guid clientId)
        {
            var SessionsPerMonth = _context.ClientTrainerEnrollments
                .Where(e => e.ClientId == clientId && e.StartDate.Year == DateTime.Now.Year)
                .GroupBy(e => e.StartDate.Month)
                .Select(g => new { Month = g.Key, Count = g.Count() });

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


        public List<ClientCoursesDto> GetActiveCourses(Guid clientId)
        {
            var courses = _context.ClientCourseEnrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Trainer)
                        .ThenInclude(t => t.User)
                .Where(e => e.ClientId == clientId && e.Course.Active == true)
                .Select(e => new ClientCoursesDto
                {
                    CourseId = e.Course!.Id,
                    CourseTitle = e.Course!.Title,
                    TrainerName = e.Course.Trainer.User.FirstName + " " + e.Course.Trainer.User.LastName

                })
                .ToList();
            return courses;

        }

        public async Task<NextTrainingRequestDto?> GetNextTrainingAsync(Guid clientId)
        {
            var nextSession = await _context.TrainingSessions
                .Include(e => e.Trainer)
                    .ThenInclude(t => t.User)
                .Where(x => x.ClientId == clientId && x.StartDateTime >= DateTime.Now)
                .OrderBy(x => x.StartDateTime)
                .Select(x => new NextTrainingRequestDto {
                    Title = x.Title,
                    StartDateTime = x.StartDateTime,
                    TrainerName = x.Trainer != null ? x.Trainer.User.FirstName + " " + x.Trainer.User.LastName  : "Individual",
                    DurationInMinutes = x.DurationInMinutes
                })
                .FirstOrDefaultAsync();
            return nextSession;
                
        }

        public async Task<ActiveSubscriptionRequestDto?> GetActiveSubscriptionAsync(Guid clientId)
        {
            var activeSubscription = await _context.Subscriptions
                .Where(e => e.UserId == clientId && e.ExpirationDate >= DateTime.Now)
                .OrderBy(e => e.ExpirationDate)
                .Select(x => new ActiveSubscriptionRequestDto
                {
                    SubscriptionName = x.Name,
                    Description = x.Description,
                    ActivationDate = x.ActivationDate,
                    ExpirationDate = x.ExpirationDate,
                    IsActive = true
                })
                .FirstOrDefaultAsync();

            if(activeSubscription == null)
            {
                var lastSubscriptionActive = await _context.Subscriptions
                    .Where(e => e.UserId == clientId)
                    .OrderByDescending(e => e.ExpirationDate)
                     .Select(x => new ActiveSubscriptionRequestDto
                     {
                         SubscriptionName = x.Name,
                         Description = x.Description,
                         ActivationDate = x.ActivationDate,
                         ExpirationDate = x.ExpirationDate,
                         IsActive = false
                     })
                     .FirstOrDefaultAsync();
                return lastSubscriptionActive;
            }

            return activeSubscription;
        }
        
        public async Task<CourseDetailsResponseDto> GetCourseDetailsAsync(Guid courseId)
        {
            var details = await _context.Courses
                .Include(t => t.Trainer)
                    .ThenInclude(t => t.User)
                .Where(e => e.Id == courseId)
                .Select(e => new CourseDetailsResponseDto
                {
                    Title = e.Title,
                    Description = e.Description,
                    Price = e.Price,
                    MaxParticipants = e.MaxParticipants,
                    TrainerName = e.Trainer.User.FirstName + " " + e.Trainer.User.LastName
                })
                .FirstOrDefaultAsync();
            return details;

        }

        public async Task<List<TrainerIntervalResponseDto>> GetAvailableTrainerIntervals(Guid clientId, DateTime date)
        {
            // 1. Găsim toți trainerii la care clientul e înscris activ
            var trainers = await _context.ClientTrainerEnrollments
                .AsNoTracking()
                .Include(e => e.Trainer)
                    .ThenInclude(t => t.TrainingSessions)
                .Include(e => e.Trainer.User)
                .Where(e => e.ClientId == clientId && date <= e.EndDate)
                .Select(e => e.Trainer)
                .ToListAsync();

            var result = new List<TrainerIntervalResponseDto>();

            foreach (var trainer in trainers)
            {
                // 2. Construim începutul și sfârșitul intervalului de lucru pentru ziua cerută
                var startTime = DateTime.ParseExact(trainer.StartInterval, "HH:mm", null);
                var endTime = DateTime.ParseExact(trainer.EndInterval, "HH:mm", null);

                var dayStart = date.Date.Add(startTime.TimeOfDay);
                var dayEnd = date.Date.Add(endTime.TimeOfDay);

                // 3. Luăm sesiunile ocupate ale trainerului în acea zi
                var sessions = trainer.TrainingSessions
                    .Where(s => s.StartDateTime.Date == date.Date)
                    .Select(s => new
                    {
                        Start = s.StartDateTime,
                        End = s.StartDateTime.AddMinutes(s.DurationInMinutes)
                    })
                    .ToList();

                // 4. Construim lista de intervale disponibile (ex: pași de 30 minute)
                var availableIntervals = new List<string>();
                var slot = dayStart;

                while (slot < dayEnd)
                {
                    var nextSlot = slot.AddMinutes(60);

                    // verificăm dacă se suprapune cu o sesiune deja rezervată
                    bool isOverlapping = sessions.Any(s =>
                        slot < s.End && nextSlot > s.Start
                    );

                    if (!isOverlapping && nextSlot <= dayEnd)
                    {
                        availableIntervals.Add($"{slot:HH:mm} - {nextSlot:HH:mm}");
                    }

                    slot = nextSlot;
                }

                result.Add(new TrainerIntervalResponseDto
                {
                    TrainerId = trainer.UserId,
                    TrainerName = trainer.User.FirstName + " " + trainer.User.LastName,
                    Intervals = availableIntervals
                });
            }

            //for self session
            result.Insert(0, new TrainerIntervalResponseDto
            {
                TrainerId = null,
                TrainerName = "None",
                Intervals = new List<string>()
            });

            return result;
        }

        public async Task<bool> AddSession(AddSessionRequestDto request)
        {
            var session = new TrainingSession
            {
                Id = Guid.NewGuid(),
                ClientId = request.ClientId,
                TrainerId = request.TrainerId,
                StartDateTime = request.StartDateTime,
                DurationInMinutes = request.DurationInMinutes,
                Title = request.Title,
                Notes = request.Notes,
            };
            _context.TrainingSessions.Add(session);
            return (await _context.SaveChangesAsync())>0;

        }

        public IQueryable<TrainerCardResponseDto> GetTrainerCardInfoAsQueryable(string? specialization)
        {
            var query = _context.Trainers
                .Include(e => e.User)
                .Include(e => e.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(specialization))
            {
                query = query.Where(x => x.Specialization == specialization);
            }

            return query.Select(e => new TrainerCardResponseDto
            {
                Id = e.UserId,
                Name = e.User.FirstName + " " + e.User.LastName,
                Specialization = e.Specialization,
                Rating = e.Reviews.Count > 0 ? e.Reviews.Sum(x => x.RatingValue) / e.Reviews.Count : 0,
                Certification = e.Certification,
                PricePerHour = e.PricePerHour,
                StartInterval = e.StartInterval,
                EndInterval = e.EndInterval,
                YearsOfExperience = e.YearsOfExperience
            });
        }

        public async Task<List<string>> GetAllSpecializations()
        {
            var result = await _context.Trainers
                .Select(x => x.Specialization)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();

            return result;
                
        }

        public async Task<bool> AddEnrollment(EnrollmentClientTrainerModelDto model)
        {
            ClientTrainerEnrollment enrollment = new ClientTrainerEnrollment { 
                Id = Guid.NewGuid(),
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ClientId = model.ClientId,
                TrainerId = model.TrainerId,
            };

            _context.ClientTrainerEnrollments.Add(enrollment);
            var result = (await _context.SaveChangesAsync()) > 0;
            return result;


        }

        public async Task<List<TrainerCardResponseDto>> GetMyTrainers(Guid clientId)
        {
            var result = await _context.ClientTrainerEnrollments
                .Include(e => e.Trainer)
                    .ThenInclude(e => e.User)
                .Include(e => e.Trainer.Reviews)
                .Where(e => e.ClientId == clientId)
                .OrderByDescending(x => x.EndDate)
                .Select(e => new TrainerCardResponseDto
                {
                    Id = e.TrainerId.Value,
                    Name = e.Trainer.User.FirstName + " " + e.Trainer.User.LastName,
                    Specialization = e.Trainer.Specialization,
                    Rating = e.Trainer.Reviews.Count > 0 ? e.Trainer.Reviews.Sum(x => x.RatingValue) / e.Trainer.Reviews.Count : 0,
                    Certification = e.Trainer.Certification,
                    PricePerHour = e.Trainer.PricePerHour,
                    StartInterval = e.Trainer.StartInterval,
                    EndInterval = e.Trainer.EndInterval,
                    YearsOfExperience = e.Trainer.YearsOfExperience
                })
                .ToListAsync();
            return result;
        }

        public IQueryable <CoursesCardResponseDto > GetCourses(string? searchCourse)
        {
            var query = _context.Courses
                .Include(e => e.Trainer)
                    .ThenInclude(e => e.User)
                .Include(e => e.ClientCourseEnrollments)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchCourse))
            {
                searchCourse = searchCourse.ToLower();
                query = query.Where(e => e.Title.ToLower().Contains(searchCourse));
            }

            var result = query
                .Select(e => new CoursesCardResponseDto
                {
                    CourseId = e.Id,
                    Title = e.Title,
                    Description = e.Description,
                    Price = e.Price,
                    MaxParticipants = e.MaxParticipants,
                    TrainerName = e.Trainer.User.FirstName + " " + e.Trainer.User.LastName,
                    Active = e.Active,
                    TotalParticipants = e.ClientCourseEnrollments.Count
                })
                ;

            return result;
        }

        public async Task<string> EnrollmentCourse(EnrollmentCourseDto model)
        {
            var isEnrolled = await _context.ClientCourseEnrollments.FirstOrDefaultAsync(x => x.ClientId == model.ClientId);
            if (isEnrolled != null) {
                return "Failed";
            }
            ClientCourseEnrollment enroll = new ClientCourseEnrollment
            {
                Id = Guid.NewGuid(),
                EnrollmentDate = DateTime.Now,
                ClientId = model.ClientId,
                CourseId = model.CourseId
            };
            _context.ClientCourseEnrollments.Add(enroll);
            var result = (await _context.SaveChangesAsync()) > 0;
            return "Success";

        }

        public async Task<SettingsClientInfoDto> GetInfoUser(Guid ClientId)
        {
            var user = await _context.Users
                .Where(x => x.Id == ClientId)
                .Select(x => new SettingsClientInfoDto
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Address = x.Address,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    UserType = x.UserType,
                })
                .FirstOrDefaultAsync();
            return user;
                
               
          
        }

        public async Task<bool> EditProfile(SettingsClientInfoDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (user != null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                _context.Users.Update(user);
                var result = (await _context.SaveChangesAsync()) > 0;
                return result;
            }
            return false;
        }

        public async Task<bool> LeaveReview(LeaveReviewModelDto model)
        {
            Review newReview = new Review
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now,
                RatingValue = model.RatingValue,
                Comment = model.Comment,
                ClientId = model.ClientId,
                TrainerId = model.TrainerId,
            };

            _context.Reviews.Add(newReview);
             var result = (await _context.SaveChangesAsync()) > 0;
             return result;

        }

    }
}
