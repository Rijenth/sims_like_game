using System;
using System.Net;
using UnityEngine;

public class CheckServerStatus : MonoBehaviour
{
    public float NextTimeout = -1;
    private float serverOfflineTimer = -1;

    public UDPService UDP;
    public IPEndPoint serverEP;

    public GameObject UsernameInput;
    public GameObject ConnexionButton;
    public GameObject ServerStartButton;
    public GameObject ServerIPAddressButton;

    private void Awake()
    {
        SetUIComponentState();
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

                serverOfflineTimer = Time.time + 3f;
            }

            SetUIComponentState();
        };
    }

    private void Update()
    {
        if (Time.time >= serverOfflineTimer)
        {
            State.ServerIsOnline = false;
            SetUIComponentState();

            serverOfflineTimer = Time.time + 2f;
        }

        if (Time.time <= NextTimeout) return;
        
        UDP.SendUDPMessage("PING", serverEP);

        NextTimeout = Time.time + 0.5f;
    }

    private void SetUIComponentState()
    {
        UsernameInput.SetActive(State.ServerIsOnline);
        ConnexionButton.SetActive(State.ServerIsOnline);
        ServerStartButton.SetActive(!State.ServerIsOnline);
    }
}