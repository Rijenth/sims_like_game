using System;
using System.Collections.Generic;
using System.Net;
using Demos.MetaVerse;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class ServerHandler : PlayerHandler
{
    private Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>();
    

    void Awake()
    {
        // envoyer message de deconnection au autres joueurs
        if (!State.IsServer) gameObject.SetActive(false);
    }

    void Start()
    {
        Debug.Log("starting as server");
        
        PlayerData data = new PlayerData()
        {
            Position = new Vector3(250, 0, 250),
            Identifier =  $"{State.ServerIP}:{State.ServerPORT}",
            Username = State.Username
        };

        AddPlayerAvatar(data, true);

        UDP.Listen(State.ServerPORT);

        UDP.OnMessageReceived +=
            (string message, IPEndPoint sender) =>
            {
                string addr = sender.Address.ToString() + ":" + sender.Port;

                if (!Clients.ContainsKey(addr))
                {
                    Clients.Add(addr, sender);
                    Debug.Log("There are " + Clients.Count + " clients present.");
                } 

                BroadcastUDPMessage(message);

                var decodedData = JsonUtility.FromJson<PlayerData>(message);

                SyncPlayerPosition(decodedData);
            };
        Invoke("InstantiateBonus", 10);
    }

    void Update()
    {
        if (Time.time <= NextTimeout) return;
        
        var json = GeneratePlayerUDPData();
        
        BroadcastUDPMessage(json);

        NextTimeout = Time.time + 0.5f;
    }

    public void InstantiateBonus() {
        if (BonusPrefab != null) {
            Debug.Log("Instantiating bonus...");
            var bonusPosition = new Vector3(248, 0, 249);
            BonusPrefab = Instantiate(BonusPrefab, bonusPosition, Quaternion.identity);
            BonusPrefab.name = "Bonus";

            
            PlayerData bonusData = new PlayerData {
                BonusPosition = bonusPosition
            };

            string json = JsonUtility.ToJson(bonusData);
            BroadcastUDPMessage(json);
            Debug.Log($"Bonus instantiated and data sent: {json}");
        }
        else {
            Debug.LogError("BonusPrefab is not assigned in ServerHandler.");
        }
    }

    public void NotifyBonusCollected() {
        PlayerData bonusData = new PlayerData {
            BonusPosition = Vector3.zero
        };
        string json = JsonUtility.ToJson(bonusData);
        BroadcastMessage(json);
        Debug.Log($"Bonus collected and data sent: {json}");
    }

    public void BroadcastUDPMessage(string message)
    {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients)
        {
            UDP.SendUDPMessage(message, client.Value);
        }
    }
    
    
}