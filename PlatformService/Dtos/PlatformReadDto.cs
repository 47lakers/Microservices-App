namespace PlatformService.Dtos
{
    // A Dto is used for data privacy and contractual coupling
    // You should be able to change your internal models without breaking any contract
    public class PlatformReadDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Publisher { get; set; }
        
        public string? Cost { get; set; }
    }
}