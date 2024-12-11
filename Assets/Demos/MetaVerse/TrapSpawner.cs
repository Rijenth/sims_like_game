using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapSpawner : MonoBehaviour 
{
    public GameObject trapPrefab;
    public float distanceBehind = 2f;
    public float trapCooldown = 5f; 
    private float lastTrapTime = -5f;
    private TextMeshProUGUI trapMessage;
    //private IPEndPoint serverEndpoint;
    private UDPService UDP;
    ClientHandler clientMan;
    ServerHandler serverMan;

    void Start()
    {
        trapMessage = GameObject.Find("TrapMessage")?.GetComponent<TextMeshProUGUI>();
        if (trapMessage == null)
        {
            Debug.LogError("TrapMessage UI not found in the scene. Please ensure it exists and is named correctly.");
        }

        if (State.IsServer) {
            serverMan = GameObject.FindObjectOfType<ServerHandler>();
        } else {
            clientMan = GameObject.FindObjectOfType<ClientHandler>();
        }
        //serverEndpoint = new IPEndPoint(System.Net.IPAddress.Parse(State.ServerIP), State.ServerPORT);
        //UDP = GameObject.FindObjectOfType<UDPService>();
    }

    void Update() 
    {
        if (Time.time >= lastTrapTime + trapCooldown)
        {
            // Affiche le message quand le piège est disponible
            UpdateTrapMessage("Vous pouvez utiliser un piège ! [A]");
        }
        else
        {
            // Calcule le temps restant avant que le piège soit disponible
            float timeRemaining = (lastTrapTime + trapCooldown) - Time.time;
            UpdateTrapMessage($"Piège disponible dans {timeRemaining:F1} secondes !");
        }

        if (Keyboard.current.qKey.wasPressedThisFrame && 
            Time.time >= lastTrapTime + trapCooldown) 
        {
            SpawnTrap();
            lastTrapTime = Time.time; 
        }
    }

    private void SpawnTrap() 
    {
        Vector3 spawnPosition = transform.position - transform.forward * distanceBehind;
        Instantiate(trapPrefab, spawnPosition, Quaternion.identity);
        Debug.Log("Trap local spawned at position: " + spawnPosition);
        Debug.Log($"Trap spawned. Next trap available in {trapCooldown} seconds");
        SendTrapDataToServer(spawnPosition);
    }

    private void SendTrapDataToServer(Vector3 position)
    {
        // Crée un objet TrapData avec la position
        TrapData trapData = new TrapData
        {
            TrapPosition = position,
            TrapIdentifier = System.Guid.NewGuid().ToString()
        };
        // Sérialise les données en JSON
        string trapJson = JsonUtility.ToJson(trapData);
        // Envoie le JSON au serveur via UDP
        //UDP.SendUDPMessage(trapJson, serverEndpoint);

        if (State.IsServer)
        {
            serverMan.BroadcastUDPMessage(trapJson);
        }
        else
        {
            clientMan.SendUDPMessageToServer(trapJson);
        }
        Debug.Log("Trap data sent to server: " + trapJson);
    }
    private void UpdateTrapMessage(string message = "")
    {
        if (trapMessage != null)
        {
            trapMessage.text = message;
        }
    }

}