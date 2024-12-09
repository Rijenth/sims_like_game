using UnityEngine;
using UnityEngine.InputSystem;

public class TrapSpawner : MonoBehaviour 
{
    public GameObject trapPrefab;
    public float distanceBehind = 2f;
    public float trapCooldown = 5f; 
    private float lastTrapTime = -5f;

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
        Debug.Log($"Trap spawned. Next trap available in {trapCooldown} seconds");
    }
}