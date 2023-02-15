using System.ComponentModel.DataAnnotations;

namespace AntalyaTaksiAccount.Models
{
    public class AllUser
    {
        public int AllUserID { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string MailAdress { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        //1:Driver 2:passenger 3:Station
        public int UserType { get; set; }
        public int? MailVerify { get; set; }
        public int Activity { get; set; }
    }
}
