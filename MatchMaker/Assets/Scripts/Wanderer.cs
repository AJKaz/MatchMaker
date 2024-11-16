using UnityEngine;

public class Wanderer : MonoBehaviour {
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private float changeDirectionTime = 2f;

    public Vector2 areaSize = new Vector2(15f, 8f);
    public Vector2 areaOffset = Vector2.zero;

    [SerializeField] private float predictionDistance = 0.5f;
    [SerializeField] private float wallAvoidanceStrength = 1f;
    [SerializeField] private float targetAvoidanceStrength = 0.5f;

    [SerializeField] private float targetAvoidanceDistance = 3f;

    private Vector2 currentDirection;
    private Vector2 targetDirection;
    private float timer;

    // TEMP
    public Wanderer wandererToAvoid;


    void Start() {
        SetRandomDirection();
        currentDirection = targetDirection;
    }

    void Update() {
        AvoidWalls();

        if (wandererToAvoid != null) {
            AvoidTarget(wandererToAvoid.transform.position);
        }

        currentDirection = Vector2.Lerp(currentDirection, targetDirection, Time.deltaTime * turnSpeed).normalized;

        Vector3 newPosition = transform.position + (Vector3)(currentDirection * moveSpeed * Time.deltaTime);

        newPosition.x = Mathf.Clamp(newPosition.x, areaOffset.x - areaSize.x / 2, areaOffset.x + areaSize.x / 2);
        newPosition.y = Mathf.Clamp(newPosition.y, areaOffset.y - areaSize.y / 2, areaOffset.y + areaSize.y / 2);
        transform.position = newPosition;

        timer += Time.deltaTime;
        if (timer >= changeDirectionTime) {
            SetRandomDirection();
            timer = 0f;
        }
    }

    private void SetRandomDirection() {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        targetDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    private void AvoidWalls() {
        Vector3 position = transform.position;

        Vector2 avoidanceForce = Vector2.zero;

        if (position.x < areaOffset.x - areaSize.x / 2 + predictionDistance) {
            avoidanceForce += Vector2.right * wallAvoidanceStrength;
        }
        else if (position.x > areaOffset.x + areaSize.x / 2 - predictionDistance) {
            avoidanceForce += Vector2.left * wallAvoidanceStrength;
        }

        if (position.y < areaOffset.y - areaSize.y / 2 + predictionDistance) {
            avoidanceForce += Vector2.up * wallAvoidanceStrength;
        }
        else if (position.y > areaOffset.y + areaSize.y / 2 - predictionDistance) {
            avoidanceForce += Vector2.down * wallAvoidanceStrength;
        }

        if (avoidanceForce != Vector2.zero) {
            targetDirection = (targetDirection + avoidanceForce).normalized;
        }
    }

    private void AvoidTarget(Vector3 targetPosition) {
        Vector3 directionAwayFromTarget = transform.position - targetPosition;

        float distanceToTarget = directionAwayFromTarget.magnitude;

        if (distanceToTarget < targetAvoidanceDistance) {
            float avoidanceFactor = Mathf.Lerp(0f, targetAvoidanceStrength, 1f - (distanceToTarget / targetAvoidanceDistance));
            targetDirection += new Vector2(directionAwayFromTarget.x, directionAwayFromTarget.y).normalized * avoidanceFactor;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaOffset, new Vector3(areaSize.x, areaSize.y, 0));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(currentDirection * predictionDistance));
    }
}

