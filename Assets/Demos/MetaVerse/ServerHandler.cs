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
                
                if (message.Contains("TrapPosition") && message.Contains("TrapIdentifier"))
                {
                    TrapData trapData = JsonUtility.FromJson<TrapData>(message);
                    Debug.Log("debug message " + message);
                    AddTrap(trapData);
                }
                else
                {
                    // Traite les données des joueurs
                    var decodedData = JsonUtility.FromJson<PlayerData>(message);
                    SyncPlayerData(decodedData);
                }
            };
    }

    void Update()
    {
        if (Time.time <= NextTimeout) return;
        
        var json = GeneratePlayerUDPData();

        BroadcastUDPMessage(json);

        NextTimeout = Time.time + 0.5f;
    }

    private void AddTrap(TrapData trapData)
    {
        // Ajoute le piège à la liste des pièges actifs
        ActiveTraps.Add(trapData);
        Debug.Log($"New trap added at position: {trapData.TrapPosition}");

        // Diffuse les informations sur le piège à tous les clients
        string trapJson = JsonUtility.ToJson(trapData);
        // BroadcastUDPMessage(trapJson);
    }

    public void BroadcastUDPMessage(string message)
    {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients)
        {
            UDP.SendUDPMessage(message, client.Value);
        }
    }
}