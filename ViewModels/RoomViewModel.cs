using GradProj.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.ViewModels
{
    public class RoomViewModel
    {

        
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
        public Guid HotelId { get; set; }
        public virtual List<Hotel> Hotels { get; set; }
        public string ImgUrl { get; set; }
        public IFormFile File { get; set; }
    }
}
