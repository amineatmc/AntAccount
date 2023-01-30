using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace AntalyaTaksiAccount.Models
{
    public class Passenger
    {
        public int PassengerID { get; set; }
      
        //tc
        public string IdNo { get; set; }
        public DateTime Birthday { get; set; }
        public bool Pet { get; set; }
        public bool Travel { get; set; }
        public bool Disabled { get; set; }
        public bool Banned { get; set; }
        public string Lang { get; set; }
        public string Lat { get; set; }
        public DateTime Created { get; set; }
        public int? AllUserID { get; set; }
        public AllUser AllUser { get; set; }
        public int Activity { get; set; }
    }
}
