using System;
using System.Collections.Generic;

#nullable disable

namespace BaoMoiAPI.Models
{
    public partial class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
