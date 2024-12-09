using UnityEngine;

public class Trap : MonoBehaviour
{
    public float immobilizationDuration = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trap.cs marché dessus :" + other.name);

            CharacterController playerController = other.GetComponent<CharacterController>();
            if (playerController != null)
            {
                Debug.Log("Trap.cs appel de ImmobilizePlayer");
                playerController.ImmobilizePlayer(immobilizationDuration);
            }
            Destroy(gameObject);
        }
    }
}
