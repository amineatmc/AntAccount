using System.Reflection.Metadata.Ecma335;

namespace AntalyaTaksiAccount.Models
{
    public class Driver
    {
        public int DriverID { get; set; }
       
        //tc
        public string IdNo { get; set; }
        public string DriverLicenseNo { get; set; }
        public int? Rating { get; set; }
        public DateTime BirthDay { get; set; }
        public bool Pet { get; set; }
        public int StationID { get; set; }
        //public Station Station { get; set; }
        public string Ip { get; set; }
        public bool Penalty { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? RoleID { get; set; }
        //public Role Role { get; set; }
        public int? AllUserID { get; set; }
        public AllUser AllUser { get; set; }
        public int Activity { get; set; }


    }
}
