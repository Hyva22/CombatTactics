using Network;

internal class Program
{
    public static void Main(string[] args)
    {
        DebugOutput.DebugAction = Console.WriteLine;
        Network.Client.Client client = new("127.0.0.1", 22222);
        client.ConnectToServer();

        Console.ReadKey();
    }
}