using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

public class ServerHandler : MonoBehaviour
{
    public UDPService UDP;
    public int ListenPort = 25000;
    private float NextTimeout = -1;
    
    public static Dictionary<string, IPEndPoint> Clients = new Dictionary<string, IPEndPoint>(); 
    
    void Awake() {
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
                string addr = sender.Address.ToString() + ":" + sender.Port;
                
                switch (message) {
                    case "connection":
                        if (!Clients.ContainsKey(addr)) {
                            Clients.Add(addr, sender);
                        }
                        Debug.Log("There are " + Clients.Count + " clients present.");
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
    
    public void BroadcastPlayerPosition(string message) {
        foreach (KeyValuePair<string, IPEndPoint> client in Clients) {
            UDP.SendUDPMessage(message, client.Value);
        }
    }

}
