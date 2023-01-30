using System.Reflection.Metadata.Ecma335;

namespace AntalyaTaksiAccount.Models
{
    public class DriverVehicle
    {
        public int DriverVehicleID { get; set; }
        public int DeriverID { get; set; }
        public Driver Driver { get; set; }
        public int VehicleID { get; set; }
        public Vehicle Vehicle { get; set; }
        public int Activity { get; set; }
    }
}
