namespace GymFit.Models
{
    public class TrainingSession
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid? TrainerId { get; set; }
        public DateTime StartDateTime { get; set; }
        public int DurationInMinutes { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        public virtual Client Client { get; set; }
        public virtual Trainer? Trainer { get; set; }
    }
}
