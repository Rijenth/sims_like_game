using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Créer des variables static centralisé
    public string ServerIP = "127.0.0.1";
    public string ServerPort = "25000";

    public CinemachineCamera CinemachineCamera;
    public ServerHandler ServerHandler;
    public GameObject MainAvatarPrefab;
    public GameObject AvatarPrefab;
    private float NextTimeout = -1;
    private int _count_connected_player = 0;

    public List<GameObject> ConnectedPlayersAvatar = new List<GameObject>();

    private void Awake()
    {
        if (!State.IsServer) gameObject.SetActive(false);
    }

    void Start()
    {
        if (State.IsServer)
        {
            if (ConnectedPlayersAvatar.Any()) return;

            if (_count_connected_player != 0) return;
            
            AddPlayerAvatar($"Player_{ServerIP}+{ServerPort}", true);
            _count_connected_player++;
        }
        
        // Initialiser les avatars des joueurs connectés au démarrage
        foreach (var client in ServerHandler.Clients)
        {
            AddPlayerAvatar(client.Key, false); // Passer la clé ou l'identifiant du client
        }

        _count_connected_player += ServerHandler.Clients.Count;
    }

    void Update()
    {
        if (Time.time <= NextTimeout) return;

        int current_player_count = ServerHandler.Clients.Count;

        if (_count_connected_player != current_player_count)
        {
            _count_connected_player = current_player_count;

            foreach (var client in ServerHandler.Clients)
            {
                if (ConnectedPlayersAvatar.Exists(a => a.name == $"Player_{client.Key}")) continue;
                
                AddPlayerAvatar(client.Key, false);
            }
        }

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

            CinemachineCamera.Follow = avatar.transform;
            CinemachineCamera.LookAt = avatar.transform;
        }
        
        // Ajouter le controller lié aux UDP si on est pas l'active player
        // if (isActive) avatar.AddComponent<UDPCharacterController>()
        
        avatar.transform.position = new Vector3(250, 0, 250); 
        avatar.name = $"Player_{clientKey}";

        ConnectedPlayersAvatar.Add(avatar);
    }
    
    
}
