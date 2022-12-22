using Network.Game;
using Npgsql;

namespace DataBase.Persistance
{
    internal static class DatabaseQueries
    {
        private const string Host = "localhost";
        private const string Username = "postgres";
        private const string Password = "123";
        private const string Database = "Game";
        private static NpgsqlConnection? connection;

        public static void Connect()
        {
            connection = new($"Host={Host};Username={Username};Password={Password};Database={Database}");
            connection.Open();
        }

        private static string GenerateInsertString(PersistantObject obj)
        {
            string query = @$"INSERT INTO player_account(";

            var result = PersistantObjectParser.Parse(obj, out string tableName, false);
            foreach(var item in result)
            {

            }
            return "";
        }

        /// <summary>
        /// Adds the given player account to the database.
        /// </summary>
        /// <param name="playerAccount">The account to be added</param>
        /// <returns>Id of the newly created account. 0 if the creation failed.</returns>
        public static long AddPlayerAccount(PlayerAccount playerAccount)
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
                var result = command.ExecuteScalar();
                if(result == null)
                {
                    Console.WriteLine("Result is null!");
                    return 0;
                }
                return (long)result;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return 0;
            }
        }

        /// <summary>
        /// Checks if a player with the given ID already exists in the database
        /// </summary>
        /// <param name="id">ID of the player</param>
        /// <returns>true if it exists, false if not</returns>
        public static bool PlayerExistsByID(long id)
        {
            string query = @$"SELECT * FROM player_account WHERE id = @id";

            NpgsqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("id", id);

            NpgsqlDataReader reader = command.ExecuteReader();
            return reader.Read();
        }


        /// <summary>
        /// Checks if a player with the given Email already exists in the database
        /// </summary>
        /// <param name="id">ID of the player</param>
        /// <returns>true if it exists, false if not</returns>
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
