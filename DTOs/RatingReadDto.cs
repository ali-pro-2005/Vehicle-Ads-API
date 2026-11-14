namespace FinalProjectWebAPI.DTOs
{
    public class RatingReadDto
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public string RaterName { get; set; } = string.Empty; // اسم من قام بالتقييم
        public DateTime CreatedAt { get; set; }
    }
}