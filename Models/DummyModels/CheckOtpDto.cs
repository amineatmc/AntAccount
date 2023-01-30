using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntalyaTaksiAccount.Models
{
    public class CheckOtpDto
    {
        public string? Phone { get; set; }
        public int? UserID { get; set; }
        public string? OtpMessage { get; set; }
    }
}
