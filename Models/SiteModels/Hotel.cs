
using GradProj.Models.SiteModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Markup;

namespace GradProj.Models
{
    public class Hotel
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Please Enter Hotel Name!!")]
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Details { get; set; }
        public string ImgUrl { get; set; }
        public int  Counter { get; set; }//For Site Style
        public virtual City City { get; set; }
        public virtual IEnumerable<Room> Rooms { get; set; }
    }
}
