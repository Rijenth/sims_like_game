using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
    public ClientSidePlayerManager ClientSidePlayerManager;
    public UDPService UDP;
    public string ServerIP = "127.0.0.1";
    public int ServerPort = 25000;

    private float NextTimeout = -1;
    private IPEndPoint ServerEndpoint;
    
    void Awake() {
        if (State.IsServer) {
            gameObject.SetActive(false);
        }
    }
    
    void Start()
    {
        Debug.Log("starting as client");
        
        UDP.InitClient();

        ServerEndpoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);
        
        // On recoit la position de tout les joueurs
        UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            
        };
    }

    // Update is called once per frame
    void Update()
    {
        // get player position
        
        if (Time.time > NextTimeout) {
            // Si on ne reçoit plus ce message, on supprime le joueur de la liste coté serveur
            UDP.SendUDPMessage("connection", ServerEndpoint);
            NextTimeout = Time.time + 0.5f;
        }
    }
}
