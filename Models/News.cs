using System;
using System.Collections.Generic;

#nullable disable

namespace BaoMoiAPI.Models
{
    public partial class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Img { get; set; }
        public DateTime? CreateAt { get; set; }
        public string CreateBy { get; set; }
        public string Description { get; set; }
        public int CateId { get; set; }
        public string Content { get; set; }

        public virtual Category Cate { get; set; }
    }
}
