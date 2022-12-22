using Network.Attributes;
using System;

namespace Network.Game
{
    [Table("player_account")]
    public class PlayerAccount : PersistantObject
    {
        [Column("email")] public string email;
        [Column("account_name")] public string accountName;
        [Column("password")] public string password;
        [Column("first_name")] public string firstName;
        [Column("sur_name")] public string surName;
        [Column("birthday")] public DateTime birthDay;
        [Column("create_date")] public DateTime creationDate;
        [Column("last_login")] public DateTime lastLogin;
    }
}
