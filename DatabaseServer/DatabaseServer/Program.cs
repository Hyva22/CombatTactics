using DataBase.Persistance;
using Network;
using Network.Game;
using System.Text.Json;

namespace DatabaseServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            DebugOutput.DebugAction = Console.WriteLine;
            Network.DatabaseServer databaseServer = new();
            DatabaseQueries.Connect();

            Thread cin = new(new ThreadStart(Cin));
            cin.Start();
        }

        private static void Cin()
        {
            string input;
            while (true)
            {
                input = Console.ReadLine() ?? "";
                if(input == "add")
                {
                    InsertTest();
                }
            }
        }

        private static void InsertTest()
        {

            PlayerAccount player = new PlayerAccount()
            {
                id = 1,
                accountName = "Test",
                email = "tt@e.de",
                password = "123"
            };

            var result = DatabaseQueries.Insert(player);

            Console.WriteLine(result);
        }
    }
}