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
                if(input == "f")
                {
                    Console.WriteLine(DatabaseQueries.PlayerExistsByEmail("t@t.de"));
                }
                if(input == "g")
                {
                    Console.WriteLine(DatabaseQueries.PlayerExistsByID(1));
                }
            }
        }
    }
}