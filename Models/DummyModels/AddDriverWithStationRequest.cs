using Microsoft.Identity.Client;

namespace AntalyaTaksiAccount.Models.DummyModels
{
    public class AddDriverWithStationRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MailAddress { get; set; }
        public string Phone { get; set; }
        public string IdNo { get; set; }
        public DateTime Birthday { get; set; }
        public string DriverLicenseNo { get;set; }
        public int? StationID { get; set; }
    }
}
