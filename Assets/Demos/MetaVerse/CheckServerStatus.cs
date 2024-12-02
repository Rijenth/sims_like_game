using System;
using System.Net;
using UnityEngine;

public class CheckServerStatus : MonoBehaviour
{
    public UDPService UDP;
    public float NextTimeout = -1;
    public IPEndPoint serverEP;

    public GameObject UsernameInput;
    public GameObject ConnexionButton;

    private void Awake()
    {
        UsernameInput.SetActive(false);
        ConnexionButton.SetActive(false);
    }

    void Start()
    {
        serverEP = new IPEndPoint(IPAddress.Parse(State.ServerIP), State.ServerPORT);
        UDP.InitClient();
        
        UDP.OnMessageReceived += (string message, IPEndPoint sender) =>
        {
            if (message == "PONG")
            {
                State.ServerIsOnline = true;
                UsernameInput.SetActive(true);
                ConnexionButton.SetActive(true);
            }
        };
    }

    private void Update()
    {
        if (State.ServerIsOnline) return;
        
        if (Time.time <= NextTimeout) return;
        
        Debug.Log("pinging server");
        
        UDP.SendUDPMessage("PING", serverEP);

        NextTimeout = Time.time + 0.5f;
    }
}