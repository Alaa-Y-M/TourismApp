using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.SiteModels
{
    public class Room
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string RoomNumber { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public int NumberOfBeds { get; set; }
        [Required]
        public bool IsBusy { get; set; }
        public virtual IEnumerable<Booking> Bookings { get; set; }
        public string ImgUrl { get; set; }
        [ForeignKey("Hotel")]
        public Guid HotelID { get; set; }
        public virtual Hotel Hotel { get; set; }
    }
}
