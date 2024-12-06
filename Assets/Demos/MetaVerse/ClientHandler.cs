using System.Collections.Generic;
using System.Net;
using Demos.MetaVerse;
using Unity.Cinemachine;
using UnityEngine;
using Player = Demos.MetaVerse.Player;

public class ClientHandler : PlayerHandler
{
    private IPEndPoint ServerEndpoint;

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
            var decodedData = JsonUtility.FromJson<PlayerData>(message);

            SyncPlayerPosition(decodedData);
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
}