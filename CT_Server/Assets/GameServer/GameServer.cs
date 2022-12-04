using Network;
using Network.Server;
using System;
using UnityEngine;

public class GameServer : MonoBehaviour
{
    [SerializeField] int maxConnection;
    [SerializeField] int port;

    [Header("Database Client")]
    [SerializeField] string databaseServerIP;
    [SerializeField] int databaseServerPort;

    private Server server;
    private Network.Client.Client client;

    private void Awake()
    {
        DebugOutput.DebugAction = Debug.Log;
        client = new(databaseServerIP, databaseServerPort);
        client.ConnectToServer();
    }
}
