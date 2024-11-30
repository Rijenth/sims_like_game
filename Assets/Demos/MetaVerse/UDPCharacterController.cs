using UnityEngine;

public class UDPCharacterController : MonoBehaviour
{
    public float WalkSpeed = 3;
    public float RotateSpeed = 250;
    private Rigidbody rb;
    private Animator anim;

    public Vector3 TargetPosition = Vector3.zero;
    private bool isMoving = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody is missing on the character!");
        }
    }

    void FixedUpdate()
    {
        if (!isMoving) return;

        MoveTowardsTarget();
    }

    public void SetMovement(Vector3 newTargetPosition)
    {
        TargetPosition = newTargetPosition;
        isMoving = true;
    }

    private void MoveTowardsTarget()
    {
        Vector3 direction = (TargetPosition - transform.position).normalized;

        float distanceToTarget = Vector3.Distance(transform.position, TargetPosition);
        if (distanceToTarget < 0.1f)
        {
            isMoving = false;

            if (anim) anim.SetFloat("Walk", 0);

            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, RotateSpeed * Time.fixedDeltaTime));

        Vector3 movement = direction * WalkSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        if (!anim) return;

        anim.SetFloat("Walk", 1);
    }
}