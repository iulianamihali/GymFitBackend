using GymFit.Data;
using GymFit.DTOs;
using GymFit.Models;
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

    }
}
