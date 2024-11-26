using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    public UDPService UDP;
    public string ServerIP = "127.0.0.1";
    public int ServerPort = 25000;
    
    private float NextTimeout = -1;
    private IPEndPoint ServerEndpoint;

    // Pour afficher tout les joueurs
    public static List<GameObject> Players = new List<GameObject>();
    
    // Comment montrer aux autres que je bouge ?

    void Awake()
    {
        if (State.IsServer)
        {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        Debug.Log("starting as client");

        UDP.InitClient();

        ServerEndpoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
        
        UDP.OnMessageReceived += (string message, IPEndPoint sender) =>
        {
            // Décoder les données reçues
            /*
            var decodedData = JsonUtility.FromJson<PlayerData>(message);
            var playerIdentifier = decodedData.Identifier;
            var playerPosition = decodedData.Position;
            var playerUsername = decodedData.Username;

            Debug.Log("[Client] Bienvenue " + playerUsername);
            */

            /*
            // Vérifier si le joueur existe déjà dans la liste
            var existingPlayer = Players.Find(player => player.name == playerName);

            if (existingPlayer != null)
            {
                // Si le joueur existe, mettre à jour sa position
                existingPlayer.transform.position = playerPosition;
                Debug.Log($"[Client] Updated position for player: {playerName}");
            }
            else
            {
                // Si le joueur n'existe pas, l'ajouter
                AddPlayerAvatar(playerName, playerPosition, false); // Par défaut, pas le personnage principal
                Debug.Log($"[Client] Added new player: {playerName}");
            }
            */
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time <= NextTimeout) return;

        PlayerData playerData = new PlayerData
        {
            Username = State.Username,
            // position ?
            // identifier ?
        };
        
        string json = JsonUtility.ToJson(playerData);
        
        // Si on ne reçoit plus ce message, on supprime le joueur de la liste coté serveur
        UDP.SendUDPMessage(json, ServerEndpoint);
        NextTimeout = Time.time + 0.5f;
    }
}