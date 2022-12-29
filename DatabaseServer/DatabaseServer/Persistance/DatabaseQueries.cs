using Network.Attributes;
using Network.Game;
using Npgsql;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace DataBase.Persistance
{
    internal static class DatabaseQueries
    {
        private const string Host = "localhost";
        private const string Username = "postgres";
        private const string Password = "123";
        private const string Database = "Game";
        private static NpgsqlConnection? connection;

        /// <summary>
        /// Connects the static class to the database
        /// </summary>
        public static void Connect()
        {
            connection = new($"Host={Host};Username={Username};Password={Password};Database={Database}");
            connection.Open();
        }

        public static Dictionary<string, object> GetData<C>(string field, object value)
        {
            string tableName = PersistantObjectParser.GetTableName<C>();
            string query = @$"SELECT * FROM {tableName} WHERE {field} = @{field}";
            NpgsqlCommand command = new(query, connection);
            command.Parameters.AddWithValue(field, value);

            Dictionary<string, object> data = new();

            NpgsqlDataReader reader;
            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return data;
            }

            if (!reader.Read())
                return data;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string dataName = reader.GetName(i);
                object dataValue = reader.GetValue(i);
                if (dataValue == DBNull.Value)
                    dataValue = null;
                data.Add(dataName, dataValue);
            }

            return data;
        }

        private static T Map<T>(Dictionary<string, object> data, Dictionary<Column, FieldInfo> fields, Func<T> factory)
        {
            T t = factory();
            foreach(var field in fields)
            {
                var name = field.Key.Name;
                object? value;
                data.TryGetValue(name, out value);
                if(value != null)
                    field.Value.SetValue(t, value);
            }

            return t;
        }

        public static T Find<T>(string field, object value, Func<T> factory)
        {
            var data = GetData<T>(field, value);
            var fields = PersistantObjectParser.GetAttributes<T, Column>();
            var result = Map<T>(data, fields, factory);

            return result;
        }

        private static string GenerateInsertString(PersistantObject obj)
        {
            var tabledata = PersistantObjectParser.Parse(obj, true);
            var keys = tabledata.Data.Keys.ToList();
            var keyString = string.Join(", ", keys);
            var valueString = "@" + string.Join(", @", keys);
            string query = @$"INSERT INTO {tabledata.TableName}({keyString}) VALUES({valueString}) RETURNING {tabledata.PrimaryKey.Item1};";
            return query;
        }

        public static long Insert(PersistantObject obj)
        {
            string query = GenerateInsertString(obj);
            NpgsqlCommand command = new(query, connection);

            var data = PersistantObjectParser.Parse(obj, true).Data;

            foreach(var item in data)
            {
                command.Parameters.AddWithValue(item.Key, item.Value ?? DBNull.Value);
            }

            try
            {
                var result = command.ExecuteScalar();
                if (result == null)
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
        /// <param dataName="id">ID of the player</param>
        /// <returns>true if it exists, false if not</returns>
        public static bool PlayerExistsByID(long id)
        {
            string query = @$"SELECT * FROM player_account WHERE id = @id";

            NpgsqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("id", id);

            using NpgsqlDataReader reader = command.ExecuteReader();
            return reader.Read();
        }


        /// <summary>
        /// Checks if a player with the given Email already exists in the database
        /// </summary>
        /// <param dataName="id">ID of the player</param>
        /// <returns>true if it exists, false if not</returns>
        public static bool PlayerExistsByEmail(string email)
        {
            string query = @$"SELECT * FROM player_account WHERE email = @email";

            NpgsqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("email", email);

            using NpgsqlDataReader reader = command.ExecuteReader();
            bool found = reader.Read();

            return found;
        }
    }
}
