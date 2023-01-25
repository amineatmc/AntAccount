using System.Reflection.Metadata.Ecma335;

namespace AntalyaTaksiAccount.Models
{
    public class VehicleOwner
    {
        public int VehicleOwnerID { get; set; }
        public string OwnerName { get; set; }
        public int VehicleID { get; set; }
        public Vehicle Vehicle { get; set; }
        public int Activity { get; set; }
    }
}
