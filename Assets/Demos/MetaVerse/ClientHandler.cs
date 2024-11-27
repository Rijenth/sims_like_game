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
            var username = decodedData.Username;
            var position = decodedData.Position;
            
            var existingPlayer = Players.Find(player => player.GetComponent<Player>().Username == username);

            if (!existingPlayer)
            {
                bool isMainCharacter = username == State.Username;

                var newPlayer = AddPlayerAvatar(decodedData, isMainCharacter);
                Players.Add(newPlayer);
            }
            else if (username != State.Username)
            {
                existingPlayer.transform.position = position;
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time <= NextTimeout) return;

        var avatar = Players.Find(player => player.GetComponent<Player>().Username == State.Username);

        var username = State.Username;
        var position = new Vector3(250, 0, 250);

        if (avatar)
        {
            var player = avatar.GetComponent<Player>();

            if (player)
            {
                username = player.Username;
                position = player.Position;
            }  
        }
        
        PlayerData playerData = new PlayerData
        {
            Username = username,
            Position = position,
        };

        string json = JsonUtility.ToJson(playerData);

        UDP.SendUDPMessage(json, ServerEndpoint);
        NextTimeout = Time.time + 0.5f;
    }
}