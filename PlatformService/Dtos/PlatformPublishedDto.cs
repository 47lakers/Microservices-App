using System.ComponentModel.DataAnnotations;

namespace PlatformService.Dtos
{

    public class PlatformPublishedDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? Event { get; set; }
    }
}