using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ServerHandler : PlayerHandler
{
    private IPEndPoint ServerEndpoint;
    
    private Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>();
    private List<TrapData> ActiveTraps = new List<TrapData>();


    void Awake()
    {
        // envoyer message de deconnection au autres joueurs
        if (!State.IsServer) gameObject.SetActive(false);
    }

    void Start()
    {
        Debug.Log("server ip address : " + State.ServerIP);
        Debug.Log("starting as server");
        
        ServerEndpoint = new IPEndPoint(IPAddress.Parse(State.ServerIP), State.ServerPORT);

        PlayerData data = new PlayerData()
        {
            Position = new Vector3(250, 0, 250),
            Identifier = $"{State.ServerIP}:{State.ServerPORT}",
            Username = State.Username
        };

        AddPlayerAvatar(data, true);

        UDP.Listen(State.ServerPORT);

        UDP.OnMessageReceived +=
            (string message, IPEndPoint sender) =>
            {
                string addr = sender.Address.ToString() + ":" + sender.Port;

                if (!Clients.ContainsKey(addr)) Clients.Add(addr, sender);

                BroadcastUDPMessage(message);

                var decodedData = JsonUtility.FromJson<PlayerData>(message);

                if (!decodedData.IsOnline) Clients.Remove(addr);

                SyncPlayerData(decodedData);
            };
    }

    void Update()
    {
        if (Time.time <= NextTimeout) return;
        
        var json = GeneratePlayerUDPData();

        BroadcastUDPMessage(json);

        NextTimeout = Time.time + 0.5f;
    }

    public void BroadcastUDPMessage(string message)
    {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients)
        {
            UDP.SendUDPMessage(message, client.Value);
        }
    }
}