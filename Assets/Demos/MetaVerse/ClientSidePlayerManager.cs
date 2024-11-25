using System.Collections.Generic;
using UnityEngine;

public class ClientSidePlayerManager : MonoBehaviour
{
    public List<GameObject> Players = new List<GameObject>();
    
    private void Awake()
    {
        if (State.IsServer) gameObject.SetActive(false);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
