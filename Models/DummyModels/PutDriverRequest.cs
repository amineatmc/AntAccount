namespace AntalyaTaksiAccount.Models.DummyModels
{
    public class PutDriverRequest
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public int DriverID { get; set; }
        public string? MailAdress { get;  set; }
        public string? Phone { get;  set; }
        public string? DriverLicenseNo { get;  set; }
        public DateTime? BirthDay { get;  set; }
        public bool? Pet { get;  set; }
        public bool? Penalty { get;  set; }
        public string? IDNo { get;  set; }
    }
}
