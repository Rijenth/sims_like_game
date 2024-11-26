using System;
using System.Collections.Generic;
using System.Net;
using Demos.MetaVerse;
using UnityEngine;
using UnityEngine.UI;

public class ServerHandler : MonoBehaviour
{
    public UDPService UDP;
    public ServerSidePlayerManager ServerSidePlayerManager;

    public int ListenPort = 25000;
    private float NextTimeout = -1;

    public static Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>();

    void Awake()
    {
        if (!State.IsServer) gameObject.SetActive(false);
    }

    void Start()
    {
        Debug.Log("starting as server");

        UDP.Listen(ListenPort);

        UDP.OnMessageReceived +=
            (string message, IPEndPoint sender) =>
            {
                string addr = sender.Address.ToString() + ":" + sender.Port;

                if (!Clients.ContainsKey(addr))
                {
                    Clients.Add(addr, sender);
                    Debug.Log("There are " + Clients.Count + " clients present.");
                }

                var decodedData = JsonUtility.FromJson<PlayerData>(message);
                var playerUsername = decodedData.Username;
                
                var existingPlayer = ServerSidePlayerManager.PlayersGameObjects.Find(player => 
                {
                    var playerComponent = player.GetComponent<Player>();
                    
                    return playerComponent && playerComponent.Username == playerUsername;
                });
                
                if (!existingPlayer)
                {
                    existingPlayer = ServerSidePlayerManager.AddPlayerAvatar(addr, playerUsername, false);

                    Debug.Log($"Added new player: {playerUsername}");

                    return;
                }

                var player = existingPlayer.GetComponent<Player>();

            };
    }

    // implémenter ceci
    public void BroadcastUDPMessage(string message)
    {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients)
        {
            UDP.SendUDPMessage(message, client.Value);
        }
    }
}