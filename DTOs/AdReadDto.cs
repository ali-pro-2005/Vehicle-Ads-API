namespace FinalProjectWebAPI.DTOs
{
    public class AdReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }

        // بيانات الناشر (للعرض فقط)
        public string SellerName { get; set; }
        public string? BranchCity { get; set; }
    }
}