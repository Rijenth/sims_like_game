using UnityEngine;

public class UDPCharacterController : MonoBehaviour
{
    public float WalkSpeed = 3;
    public float RotateSpeed = 250;
    private Rigidbody rb;
    private Animator anim;

    public Vector3 TargetPosition = Vector3.zero;
    private bool isMoving = false;

    public float StoppingDistance = 0.1f;
    public float DecelerationFactor = 0.5f;

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
        /*
            On ignore si on reçoit un ordre de mouvement mais que seul l'axe y à changer.
            risque d'erreur avec la rotation des persos car tout les persos
            spawn en (250, 0, 250) au démarrage.
         */
        if (Mathf.Abs(newTargetPosition.x - TargetPosition.x) < 0.01f &&
            Mathf.Abs(newTargetPosition.z - TargetPosition.z) < 0.01f)
        {
            return;
        }

        TargetPosition = newTargetPosition;
        isMoving = true;
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

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRotation, RotateSpeed * Time.fixedDeltaTime));

        Vector3 movement = direction * WalkSpeed * speedMultiplier * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        if (!anim) return;

        anim.SetFloat("Walk", speedMultiplier);
    }
}