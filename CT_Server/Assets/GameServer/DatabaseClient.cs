using Network.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseClient
{
    private Client client;

    public void Connect(string ip, int port)
    {
        client = new(ip, port);
        client.ConnectToServer();
    }
}
