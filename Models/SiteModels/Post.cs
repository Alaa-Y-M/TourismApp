using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProj.Models.SiteModels
{
    public class Post
    {
        public Guid Id { get; set; }
        public string PostBody { get; set; }
        public string PostReply { get; set; }
        public decimal? Like { get; set; }
        public string UserLikeName { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
