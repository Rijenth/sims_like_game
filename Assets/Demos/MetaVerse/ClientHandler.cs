using System.Collections.Generic;
using System.Net;
using Demos.MetaVerse;
using Unity.Cinemachine;
using UnityEngine;
using Player = Demos.MetaVerse.Player;

public class ClientHandler : MonoBehaviour
{
    public UDPService UDP;
    public string ServerIP = "127.0.0.1";
    public int ServerPort = 25000;

    private float NextTimeout = -1;
    private IPEndPoint ServerEndpoint;

    public CinemachineCamera cinemachineCamera;
    public GameObject MainAvatarPrefab;
    public GameObject AvatarPrefab;

    // Pour afficher tout les joueurs
    public static List<GameObject> Players = new List<GameObject>();

    void Awake()
    {
        if (State.IsServer) gameObject.SetActive(false);
    }

    void Start()
    {
        Debug.Log("starting as client");

        UDP.InitClient();

        ServerEndpoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);

        UDP.OnMessageReceived += (string message, IPEndPoint sender) =>
        {
            Debug.Log("Message reçu");
            var decodedData = JsonUtility.FromJson<PlayerData>(message);
            var username = decodedData.Username;
            var position = decodedData.Position;

            Debug.Log("Username : " + username);

            var existingPlayer = Players.Find(player => player.GetComponent<Player>().Username == username);

            if (!existingPlayer)
            {
                bool isMainCharacter = username == State.Username;
                Debug.Log("on a pas trouvé de joueur donc on en crée un qui sera main charac ? " + isMainCharacter);

                var newPlayer = AddPlayerAvatar(decodedData, isMainCharacter);
                Players.Add(newPlayer);
                Debug.Log($"[Client] Added new player: {username}");
            }
            else if (username != State.Username)
            {
                Debug.Log("cest un autre joueur, donc on le bouge");
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


    public GameObject AddPlayerAvatar(PlayerData data, bool isMainCharacter)
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
        avatar.name = data.Username;

        Player playerComponent = avatar.AddComponent<Player>();
        playerComponent.Identifier = data.Identifier;
        playerComponent.Avatar = avatar;
        playerComponent.Username = data.Username;

        Players.Add(avatar);

        return avatar;
    }
}