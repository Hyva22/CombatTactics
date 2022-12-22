using Network;
using Network.Game;
using Network.Server;

class Program
{
    public static void Main()
    {
        PlayerAccount player = new()
        {
            id = 1,
            email = "t@e.de",
            password = "123",
        };
        var res = PersistantObjectParser.Parse(player, out string tableName, false);
        Console.WriteLine(tableName);
        foreach (var item in res)
        {
            Console.WriteLine(item);
        }
    }
}