using System;
using System.Collections.Generic;
using System.Linq;
using Demos.MetaVerse;
using Unity.Cinemachine;
using UnityEngine;

public class ServerSidePlayerManager : MonoBehaviour
{
    // Créer des variables static centralisé
    public string serverIP = "127.0.0.1";
    public string serverPort = "25000";

    public CinemachineCamera cinemachineCamera;
    public ServerHandler ServerHandler;
    public GameObject MainAvatarPrefab;
    public GameObject AvatarPrefab;
    private float NextTimeout = -1;
    private int countConnectedPlayer = 0;

    public List<GameObject> PlayersGameObjects = new List<GameObject>();

    private void Awake()
    {
        if (!State.IsServer) gameObject.SetActive(false);
    }

    void Start()
    {
        if (State.IsServer)
        {
            if (PlayersGameObjects.Any()) return;

            if (countConnectedPlayer != 0) return;
            
            AddPlayerAvatar($"Player_{serverIP}+{serverPort}", true);
            countConnectedPlayer++;
        }
        
        // Initialiser les avatars des joueurs connectés au démarrage
        foreach (var client in ServerHandler.Clients)
        {
            AddPlayerAvatar(client.Key, false);
        }

        countConnectedPlayer += ServerHandler.Clients.Count;
    }

    void Update()
    {
        if (Time.time <= NextTimeout) return;

        int currentPlayerCount = ServerHandler.Clients.Count;

        if (countConnectedPlayer != currentPlayerCount)
        {
            countConnectedPlayer = currentPlayerCount;

            foreach (var client in ServerHandler.Clients)
            {
                if (PlayersGameObjects.Exists(a => a.name == $"Player_{client.Key}")) continue;
                
                AddPlayerAvatar(client.Key, false);
            }
        }
        
        // objectif : 
        // Permettre l'instanciation des personnages chez les clients,
        // leur position actuel
    
        if (Time.time <= NextTimeout) return;
        
        
        /*
        foreach (Player player in Players)
        {
            // identifiant du joueur | position x | position y | position z
            float x = player.transform.position.x;
            float y = player.transform.position.y;
            float z = player.transform.position.z;
            string characterPosition = $"Player_{x}+{y}+{z}";

            PlayerPosition playerPosition = new PlayerPosition
            {
                //identifier = player
                Position = transform.position
            };
            
            string json = JsonUtility.ToJson(playerPosition);

            
            ServerHandler.BroadcastUDPMessage(characterPosition);
        }
        */
        
        
        NextTimeout = Time.time + 0.5f;
    }
    
    void AddPlayerAvatar(string clientKey, bool isMainCharacter)
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
        avatar.name = $"Player_{clientKey}";

        Player playerComponent = avatar.AddComponent<Player>();
        playerComponent.Identifier = clientKey;
        playerComponent.Avatar = avatar;

        PlayersGameObjects.Add(avatar);
    }
}
