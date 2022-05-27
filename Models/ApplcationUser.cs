using Microsoft.AspNetCore.Identity;

namespace GradProj.Models
{
    public class ApplcationUser:IdentityUser
    {
        public string ImgUrl { get; set; }
        public string Job { get; set; }
    }
}
