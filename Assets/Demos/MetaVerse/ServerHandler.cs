using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ServerHandler : MonoBehaviour
{
    public UDPService UDP;
    public int ListenPort = 25000;
    
    public Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>(); 
    
    void Awake() {
        // Desactiver mon objet si je ne suis pas le serveur
        Debug.Log("server handler : " + State.IsServer);

        
        if (!State.IsServer) {
            gameObject.SetActive(false);
        }
    }
    
    void Start()
    {
        Debug.Log("starting as server");
        
        UDP.Listen(ListenPort);

        UDP.OnMessageReceived +=  
            (string message, IPEndPoint sender) => {
                Debug.Log("[SERVER] Message received from " + 
                          sender.Address.ToString() + ":" + sender.Port 
                          + " =>" + message);
                
                switch (message) {
                    case "coucou":
                        string addr = sender.Address.ToString() + ":" + sender.Port;
                        if (!Clients.ContainsKey(addr)) {
                            Clients.Add(addr, sender);
                        }
                        Debug.Log("There are " + Clients.Count + " clients present.");

                        UDP.SendUDPMessage("welcome!", sender);
                        break;
                }
                
                //@todo : do something with the message that has arrived! 
            };
    }
    
    // implémenter ceci
    public void BroadcastUDPMessage(string message) {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients) {
            UDP.SendUDPMessage(message, client.Value);
        }
    }
}
