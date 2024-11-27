using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Demos.MetaVerse;
using Unity.Cinemachine;
using UnityEngine;

public class ServerSidePlayerManager : MonoBehaviour
{
    // Créer des variables static centralisé
    public string serverIP = "127.0.0.1";
    public int serverPort = 25000;
    private IPEndPoint ServerEndpoint;

    public CinemachineCamera cinemachineCamera;
    public ServerHandler ServerHandler;
    public GameObject MainAvatarPrefab;
    public GameObject AvatarPrefab;

    private float NextTimeout = -1;
    private int countConnectedPlayer = 0;

    public static List<GameObject> PlayersGameObjects = new List<GameObject>();

    private void Awake()
    {
        if (!State.IsServer) gameObject.SetActive(false);
    }

    void Start()
    {
        /*
        if (!State.IsServer) return;
        if (PlayersGameObjects.Any()) return;

        var addr = $"{serverIP}:{serverPort}";
        AddPlayerAvatar(addr, State.Username, true);
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        
        if (!ServerHandler.Clients.ContainsKey(addr))
        {
            ServerHandler.Clients.Add(addr, serverEndPoint);
        }

        countConnectedPlayer++;
        */
    }

    void Update()
    {
        /*
        // toutes cette classe est potentiellement useless
        if (Time.time <= NextTimeout) return;

        countConnectedPlayer = ServerHandler.Clients.Count;
        
        Debug.Log("There are " + countConnectedPlayer + " clients present.");
        
        foreach (GameObject avatar in PlayersGameObjects)
        {
            //avatar.AddComponent<Player>();
            
            Player player = avatar.GetComponent<Player>();

            PlayerData playerData = new PlayerData
            {
                Identifier = player.Identifier,
                Position = player.Position,
                Username= player.Username
            };

            string json = JsonUtility.ToJson(playerData);
            
            ServerHandler.BroadcastUDPMessage(json);
        }
        
        NextTimeout = Time.time + 0.5f;
        */
    }

    /*
    public GameObject AddPlayerAvatar(string clientKey, string playerUsername, bool isMainCharacter)
    {
        
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
        avatar.name = clientKey;

        Player playerComponent = avatar.AddComponent<Player>();
        playerComponent.Identifier = clientKey;
        playerComponent.Avatar = avatar;
        playerComponent.Username = playerUsername;

        PlayersGameObjects.Add(avatar);

        return avatar;
    }
    */
}