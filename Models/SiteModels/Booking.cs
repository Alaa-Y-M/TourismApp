using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.SiteModels
{
    public class Booking
    {
        [Key]
        public Guid Id { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CheckIn { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime CheckOut { get; set; }
        public virtual Room Room { get; set; }
        public virtual Customer Customer { get; set; }
        [Required]
        public int People { get; set; }
    }
}
