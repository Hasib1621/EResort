using System.ComponentModel.DataAnnotations;

namespace EResort_ResortAPI.Models.Dto
{
    public class ResortDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string Name { get; set; }
    }
}
