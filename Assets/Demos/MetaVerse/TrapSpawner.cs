using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrapSpawner : MonoBehaviour 
{
    public GameObject trapPrefab;
    public float distanceBehind = 2f;
    public float trapCooldown = 5f; 
private float lastTrapTime = -5f;
    //private IPEndPoint serverEndpoint;
    private UDPService UDP;
    ClientHandler clientMan;
    ServerHandler serverMan;

    void Start()
    {
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
}