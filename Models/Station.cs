using System.Reflection.Metadata.Ecma335;

namespace AntalyaTaksiAccount.Models
{
    public class Station
    {
        public int StationID { get; set; }
        public int StationNumber { get; set; }
        public string StationArea { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public bool StationStatu { get; set; }
        public bool StationAuto { get; set; }
        public string Ip { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? AllUserID { get; set; }
        public AllUser AllUser { get; set; }
        public int Activity { get; set; }
    }
}
