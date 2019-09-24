using System.ComponentModel.DataAnnotations;

namespace CleanArchTemplate.BC.Home.Domain
{
    public class Genre
    {
        public byte Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }
    }

}