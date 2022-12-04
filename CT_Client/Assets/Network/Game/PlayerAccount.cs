using System;

namespace Network.Game
{
    public class PlayerAccount : PersistantObject
    {
        public string email;
        public string accountName;
        public string password;
        public string firstName;
        public string surName;
        public DateTime birthDay;
        public DateTime creationDate;
        public DateTime lastLogin;

        //public string Email { get => email; set => email = value; }
        //public string AccountName { get => accountName; set => accountName = value; }
        //public string Password { get => password; set => password = value; }
        //public string FirstName { get => firstName; set => firstName = value; }
        //public string SurName { get => surName; set => surName = value; }
        //public DateTime BirthDay { get => birthDay; set => birthDay = value; }
        //public DateTime CreationDate { get => creationDate; set => creationDate = value; }
        //public DateTime LastLogin { get => lastLogin; set => lastLogin = value; }
    }
}
