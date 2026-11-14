namespace FinalProjectWebAPI.DTOs
{
    public class ShowroomReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LicenseNumber { get; set; }
        public string OwnerName { get; set; }
        public List<BranchReadDto> Branches { get; set; } = new();
    }
}