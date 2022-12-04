using Network;
using Network.Server;

class Program
{
    public static void Main()
    {
        DebugOutput.DebugAction = Console.WriteLine;
        Server server = new(10, 22222, 1);
        server.Start();

        while (true)
        {
            Thread.Sleep(1000);
        }
    }
}