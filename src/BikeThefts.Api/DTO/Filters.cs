using System.ComponentModel.DataAnnotations;

namespace BikeThefts.Api.DTO
{
    public class Filters
    {
        [Required]
        public string Location { get; set; }

        [Required]
        [RequiredGreaterThanZeroValidation]
        public double Distance { get; set; }
    }
}
