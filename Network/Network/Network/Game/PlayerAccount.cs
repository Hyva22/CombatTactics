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
    }
}
