using System.Collections.Generic;
using System.Net;
using Demos.MetaVerse;
using Unity.Cinemachine;
using UnityEngine;
using Player = Demos.MetaVerse.Player;

public class ClientHandler : PlayerHandler
{
    private IPEndPoint ServerEndpoint;
    public GameObject trapPrefab;
    void Awake()
    {
        if (State.IsServer) gameObject.SetActive(false);
    }

    void Start()
    {
        Debug.Log("starting as client");

        UDP.InitClient();

        ServerEndpoint = new IPEndPoint(IPAddress.Parse(State.ServerIP), State.ServerPORT);

        UDP.OnMessageReceived += (string message, IPEndPoint sender) =>
        {
            if (message.Contains("TrapPosition") && message.Contains("TrapIdentifier"))
            {
                // Décoder les données du piège
                TrapData trapData = JsonUtility.FromJson<TrapData>(message);

                // Instancier le piège localement
                SpawnTrapLocally(trapData);
            }
            else
            {
                // Décoder les données du joueur
                var decodedData = JsonUtility.FromJson<PlayerData>(message);
                SyncPlayerData(decodedData);
            }

        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time <= NextTimeout) return;

        var json = GeneratePlayerUDPData();

        UDP.SendUDPMessage(json, ServerEndpoint);

        NextTimeout = Time.time + 0.5f;
    }

    private void SpawnTrapLocally(TrapData trapData)
    {
        // Instancie un piège à la position spécifiée
        Instantiate(trapPrefab, trapData.TrapPosition, Quaternion.identity);
        Debug.Log($"Trap spawned locally at position: {trapData.TrapPosition}");
    }

    public void SendUDPMessageToServer(string message)
    {
        UDP.SendUDPMessage(message, ServerEndpoint);
    }


    private void OnDestroy()
    {
        var json = GeneratePlayerUDPData(false);
        
        UDP.SendUDPMessage(json, ServerEndpoint);

        UDP.CloseUDP();
    }
}