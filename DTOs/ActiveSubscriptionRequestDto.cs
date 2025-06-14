namespace GymFit.DTOs
{
    public class ActiveSubscriptionRequestDto
    {
        public string SubscriptionName { get; set; }
        public string Description { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
