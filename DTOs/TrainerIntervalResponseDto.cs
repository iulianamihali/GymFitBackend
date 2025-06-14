namespace GymFit.DTOs
{
    public class TrainerIntervalResponseDto
    {
        public Guid? TrainerId { get; set; }
        public string TrainerName { get; set; }
        public List<string> Intervals { get; set; }


    }
}
