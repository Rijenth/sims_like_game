using UnityEngine;

public class UDPCharacterController : MonoBehaviour
{
    public float WalkSpeed = 3;
    public float RotateSpeed = 250;
    private Rigidbody rb;
    private Animator anim;

    public Vector3 TargetPosition = Vector3.zero;
    public Quaternion TargetRotation;
    private bool isMoving = false;

    public float StoppingDistance = 0.1f;
    public float DecelerationFactor = 0.5f;

    private CharacterController playerController;
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
    if (playerController != null && playerController.isImmobilized)
    {
        isMoving = false;
        return;
    }

    if (Mathf.Abs(newTargetPosition.x - TargetPosition.x) < 0.01f &&
        Mathf.Abs(newTargetPosition.z - TargetPosition.z) < 0.01f)
    {
        return;
    }

    TargetPosition = newTargetPosition;
    isMoving = true;
}

    public void SetRotation(Quaternion newRotation)
    {
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, newRotation, RotateSpeed));
    }


    private void MoveTowardsTarget()
    {
        // Ignore la composante Y lors du calcul
        Vector3 currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 targetPositionFlat = new Vector3(TargetPosition.x, 0, TargetPosition.z);

        Vector3 direction = (targetPositionFlat - currentPosition).normalized;
        float distanceToTarget = Vector3.Distance(currentPosition, targetPositionFlat);

        if (distanceToTarget < StoppingDistance)
        {
            isMoving = false;

            if (anim) anim.SetFloat("Walk", 0);

            return;
        }

        float speedMultiplier = distanceToTarget < 1f
            ? Mathf.Lerp(DecelerationFactor, 1f, distanceToTarget)
            : 1f;
        
        Vector3 movement = direction * WalkSpeed * speedMultiplier * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        if (!anim) return;

        anim.SetFloat("Walk", speedMultiplier);
    }
}