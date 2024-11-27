using System;
using System.Collections.Generic;
using System.Net;
using Demos.MetaVerse;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerData
{
    public string Identifier;
    public Vector3 Position;
    public string Username;
}

public class ServerHandler : MonoBehaviour
{
    public UDPService UDP;
    public ServerSidePlayerManager ServerSidePlayerManager;

    public CinemachineCamera cinemachineCamera;
    public GameObject MainAvatarPrefab;
    public GameObject AvatarPrefab;

    public string serverIP = "127.0.0.1";
    public int serverPort = 25000;
    private float NextTimeout = -1;

    public static Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>();
    public static List<GameObject> Players = new List<GameObject>();

    void Awake()
    {
        if (!State.IsServer) gameObject.SetActive(false);
    }

    void Start()
    {
        Debug.Log("starting as server");
        
        PlayerData data = new PlayerData()
        {
            Position = new Vector3(250, 0, 250),
            Identifier =  $"{serverIP}:{serverPort}",
            Username = State.Username
        };

        AddPlayerAvatar(data, true);

        UDP.Listen(serverPort);

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
                var username = decodedData.Username;
                var position = decodedData.Position;

                var existingPlayer = Players.Find(player => player.GetComponent<Player>().Username == username);

                if (!existingPlayer)
                {
                    var newPlayer = AddPlayerAvatar(decodedData, false);
                    Players.Add(newPlayer);
                    Debug.Log($"[Client] Added new player: {username}");
                }
                else if (username != State.Username)
                {
                    existingPlayer.transform.position = position;
                    Debug.Log($"[Client] Updated position for player: {username}");
                }
            };
    }

    void Update()
    {
        if (Time.time <= NextTimeout) return;

        var avatar = Players.Find(player => player.GetComponent<Player>().Username == State.Username);
        
        if (!avatar) return;
        
        var player = avatar.GetComponent<Player>();

        PlayerData playerData = new PlayerData
        {
            Username = player.Username,
            Position = player.Position,
            Identifier = player.Identifier
        };

        string json = JsonUtility.ToJson(playerData);

        BroadcastUDPMessage(json);

        NextTimeout = Time.time + 0.5f;
    }

    public GameObject AddPlayerAvatar(PlayerData data, bool isMainCharacter)
    {
                
        Debug.Log("[SERVER] tu est le main character ? " + isMainCharacter);
        
        GameObject avatar = (isMainCharacter)
            ? Instantiate(MainAvatarPrefab)
            : Instantiate(AvatarPrefab);

        if (isMainCharacter)
        {
            avatar.AddComponent<CharacterController>();

            cinemachineCamera.Follow = avatar.transform;
            cinemachineCamera.LookAt = avatar.transform;
        }

        // Ajouter le controller lié aux UDP si on est pas l'active player
        // if (isActive) avatar.AddComponent<UDPCharacterController>()

        avatar.transform.position = new Vector3(250, 0, 250);
        avatar.name = data.Username;

        Player playerComponent = avatar.AddComponent<Player>();
        playerComponent.Identifier = data.Identifier;
        playerComponent.Avatar = avatar;
        playerComponent.Username = data.Username;

        Players.Add(avatar);

        return avatar;
    }

    public void BroadcastUDPMessage(string message)
    {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients)
        {
            UDP.SendUDPMessage(message, client.Value);
        }
    }
}