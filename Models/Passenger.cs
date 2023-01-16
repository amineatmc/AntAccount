using System.Reflection.Metadata.Ecma335;

namespace AntalyaTaksiAccount.Models
{
    public class Passenger
    {
        public int PassengerID { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string MailAdress { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string? ProfilePhotoPath { get; set; }
        public int MailVerify { get; set; }
        public int ResetPasswordVerify { get; set; }
        public DateTime InsertedDate { get; set; }
        public int? GenderID { get; set; }
        public Gender Gender { get; set; }
        public DateTime PasswordChangeDate { get; set; }
        public int PasswordExpiration { get; set; }
        public int Activity { get; set; }

    }
}
