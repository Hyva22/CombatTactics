using Network;
using Network.Server;

class Program
{
    private static Server server;

    public static void Main()
    {
        DebugOutput.DebugAction = Console.WriteLine;
        server = new(10, 22220, 1);
        server.Start();

        Thread cin = new(Cin);
        cin.Start();

        while (true)
        {
            Thread.Sleep(1000);
        }
    }

    private static void Cin()
    {
        while(true)
        {
            string cin = Console.ReadLine();
            int clientId = int.Parse(cin);
            server.SendUDPTest(clientId);
        }
    }
}