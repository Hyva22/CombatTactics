using Network.Game;
using Npgsql;

namespace DataBase.Persistance
{
    internal static class DatabaseQueries
    {
        private static NpgsqlConnection? connection;

        public static void Connect()
        {
            connection = new("Host=localhost;Username=postgres;Password=$$BBsp0r3;Database=Game");
            connection.Open();
        }

        public static void AddPlayerAccount(string email, string password, string accountName, string firstName, string surName,
            int birthdayYear, int birthdayMonth, int birthdayDay)
        {
            if (PlayerExistsByEmail(email))
                return;

            string query = @$"INSERT INTO player_account(email, account_name, password, first_name, sur_name, birthday, create_date)
                                    VALUES(@email, @account_name, @password, @first_name, @sur_name, @birthday, @date)
                                    RETURNING id;";

            NpgsqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("email", email);
            command.Parameters.AddWithValue("account_name", accountName);
            command.Parameters.AddWithValue("password", password);
            command.Parameters.AddWithValue("first_name", firstName);
            command.Parameters.AddWithValue("sur_name", surName);
            command.Parameters.AddWithValue("birthday", new DateOnly(birthdayYear, birthdayMonth, birthdayDay));
            command.Parameters.AddWithValue("date", DateTime.Now);

            long result = (long)command.ExecuteScalar();
        }

        public static bool AddPlayerAccount(PlayerAccount playerAccount)
        {
            Console.WriteLine($"Email: {playerAccount.email}");
            string query = @$"INSERT INTO player_account(email, account_name, password, first_name, sur_name, birthday, create_date)
                                    VALUES(@email, @account_name, @password, @first_name, @sur_name, @birthday, @date)
                                    RETURNING id;";

            NpgsqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("email", playerAccount.email);
            command.Parameters.AddWithValue("account_name", playerAccount.accountName);
            command.Parameters.AddWithValue("password", playerAccount.password);
            command.Parameters.AddWithValue("first_name", playerAccount.firstName);
            command.Parameters.AddWithValue("sur_name", playerAccount.surName);
            command.Parameters.AddWithValue("birthday", playerAccount.birthDay);
            command.Parameters.AddWithValue("date", DateTime.Now);
            try
            {
                long result = (long)command.ExecuteScalar();
            }catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        public static bool PlayerExistsByID(long id)
        {
            string query = @$"SELECT * FROM player_account WHERE id = @id";

            NpgsqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("id", id);

            NpgsqlDataReader reader = command.ExecuteReader();
            return reader.Read();
        }

        public static bool PlayerExistsByEmail(string email)
        {
            string query = @$"SELECT * FROM player_account WHERE email = @email";

            NpgsqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("email", email);

            NpgsqlDataReader reader = command.ExecuteReader();
            return reader.Read();
        }

        //public static PlayerAccount? GetPlayerAccount(long id)
        //{
        //    string query = @$"SELECT * FROM player_account WHERE id = @id";

        //    NpgsqlCommand command = new(query, connection);
        //    command.Parameters.AddWithValue("id", id);

        //    NpgsqlDataReader reader = command.ExecuteReader();
        //    if(!reader.Read())
        //        return null;

        //    id = reader.GetInt64(0);
        //    string email = reader.GetString(1);
        //    string accountName = reader.GetString(2);
        //    string password = reader.GetString(3);
        //    string firstName = reader.GetString(4);
        //    string surName = reader.GetString(5);
        //    DateOnly birthDay = DateOnly.FromDateTime(reader.GetDateTime(6));
        //    DateTime creationDate = reader.GetDateTime(7);
        //    DateTime LastLogin;
        //    try
        //    {
        //        LastLogin = reader.GetDateTime(8);
        //    }
        //    catch (Exception)
        //    {
        //        LastLogin = default;
        //    }

        //    PlayerAccount playerAccount = new(id, email, accountName, password, creationDate)
        //    {
        //        FirstName= firstName,
        //        SurName= surName,
        //        BirthDay = birthDay,
        //        LastLogin = LastLogin
        //    };
        //    return playerAccount;
        //}
    }
}
