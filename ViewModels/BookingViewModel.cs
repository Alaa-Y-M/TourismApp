using GradProj.Models;
using GradProj.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.ViewModels
{
    public class BookingViewModel
    {
        public Guid Id { get; set; }
        [DataType(DataType.DateTime),Required]
        public DateTime CheckIn { get; set; }
        [DataType(DataType.DateTime),Required]
        public DateTime CheckOut { get; set; }
        [DataType(DataType.EmailAddress), Required]
        public string Email { get; set; }
        [Required]
        public int People { get; set; }
        public Guid RoomId { get; set; }
        public List<Room> Rooms { get; set; }
    }
}
