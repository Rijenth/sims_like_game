using System.Net;
using UnityEngine;

public class ClientHandler : MonoBehaviour
{
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
        
        UDP.OnMessageReceived += (string message, IPEndPoint sender) => {
            Debug.Log("[CLIENT] Message received from " + 
                      sender.Address.ToString() + ":" + sender.Port 
                      + " =>" + message);
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > NextTimeout) {
            UDP.SendUDPMessage("connection", ServerEndpoint);
            NextTimeout = Time.time + 0.5f;
        }
    }
}
