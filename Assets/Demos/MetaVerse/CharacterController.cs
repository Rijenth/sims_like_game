using UnityEngine;
using UnityEngine.InputSystem;

public enum CharacterPlayer {
  Player1
}

public class CharacterController : MonoBehaviour
{
    public CharacterPlayer Player = CharacterPlayer.Player1;
    public float WalkSpeed = 3;
    public float RotateSpeed = 250;
    Animator Anim;
    MetaverseInput inputs;
    InputAction PlayerAction;
    Rigidbody rb;

    public bool isImmobilized = false;
    private float immobilizationTimer = 0f;
    public float immobilizationDuration = 2f;

    void Start()
    {
        Anim = GetComponent<Animator>();
        inputs = new MetaverseInput();
        
        PlayerAction = inputs.Player1.Move;

        PlayerAction.Enable();

        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
      Debug.Log("isImmobilized in FixedUpdate: " + isImmobilized);

        if (isImmobilized == true)
        {
            Debug.Log("Player is immobilized, timer: " + immobilizationTimer);

            immobilizationTimer -= Time.fixedDeltaTime;

            if (immobilizationTimer <= 0f)
            {
                isImmobilized = false;
                Debug.Log("Player is no longer immobilized");
                Anim.SetFloat("Walk", 0);
            }
            return;
        }
        Vector2 vec = PlayerAction.ReadValue<Vector2>();
        Anim.SetFloat("Walk", vec.y);

        rb.MovePosition(rb.position + transform.forward * WalkSpeed * Time.fixedDeltaTime * vec.y);

        rb.MoveRotation(rb.rotation * Quaternion.AngleAxis(RotateSpeed * Time.fixedDeltaTime * vec.x, Vector3.up));
    }

    public void ImmobilizePlayer(float duration)
    {
        Debug.Log("ImmobilizePlayer called with duration: " + duration);
        isImmobilized = true;
        immobilizationTimer = duration;
        Debug.Log("ImmobilizePlayer immobilizationTimer: " + immobilizationTimer + " isImmobilized: " + isImmobilized);
    }

    void OnDisable() {
        PlayerAction.Disable();
    }
}
