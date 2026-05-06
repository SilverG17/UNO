using UnityEngine;

public class ClientBootstrap : MonoBehaviour
{
    [SerializeField] private ClientNetworkManager client;

    void Start()
    {
        client.OnMessageReceived += msg =>
        {
            Debug.Log("[GAME] RECEIVED: " + msg);
        };

        client.OnConnected += () =>
        {
            Debug.Log("[GAME] CONNECTED EVENT");

            client.Send("{\"type\":\"Ping\",\"payload\":\"hello server\"}");
        };

        client.Connect("127.0.0.1", 7777);
    }
}