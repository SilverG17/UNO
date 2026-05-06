using UnityEngine;

public class ServerBootstrap : MonoBehaviour
{
    private ServerNetworkManager server;

    void Start()
    {
        server = new ServerNetworkManager(7777);
        server.Start();

        Debug.Log("Server started inside Unity");
    }

    void OnApplicationQuit()
    {
        server?.Stop();
    }
}