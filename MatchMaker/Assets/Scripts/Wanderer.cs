using UnityEngine;

public class Wanderer : MonoBehaviour {
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float scatterSpeed = 5f;
    [SerializeField] private float matingSpeed = 2.5f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private float changeDirectionTime = 2f;
    [SerializeField] private float stoppingDistance = 1f;


    public Vector2 areaSize = new Vector2(15f, 8f);
    public Vector2 areaOffset = Vector2.zero;

    [SerializeField] private float predictionDistance = 0.5f;
    [SerializeField] private float wallAvoidanceStrength = 1f;
    [SerializeField] private float targetAvoidanceStrength = 0.5f;

    [SerializeField] private float normalAvoidanceDistance = 0.5f;
    [SerializeField] private float targetAvoidanceDistance = 2f;

    private Vector2 currentDirection;
    private Vector2 targetDirection;
    private float timer;

    public SpriteRenderer spriteRenderer;

    private bool bWander = true;
    private bool bScatter = false;

    private Vector3 targetDestination;
    private bool bGoToTargetDestination = false;
    public bool bAtTarget = false;

    void Start() {
        SetRandomDirection();
        currentDirection = targetDirection;
    }

    void Update() {
        if (bGoToTargetDestination) {
            PathToTarget();
            return;
        }

        if (!bWander) {
            return;
        }

        if (bScatter) {
            AvoidPosition(Vector3.zero, 1000f);
        }
        else {
            AvoidWalls();
        }

        for (int i = 0; i < GameManager.Instance.wandererList.Count; i++) {
            float avoidanceDistance = GameManager.Instance.IsWandererSelected(GameManager.Instance.wandererList[i]) ? targetAvoidanceDistance : normalAvoidanceDistance;
            AvoidPosition(GameManager.Instance.wandererList[i].transform.position, avoidanceDistance);
        }

        currentDirection = Vector2.Lerp(currentDirection, targetDirection, Time.deltaTime * turnSpeed).normalized;

        Vector3 newPosition = transform.position + (Vector3)(currentDirection * moveSpeed * Time.deltaTime);

        if (!bScatter) {
            newPosition.x = Mathf.Clamp(newPosition.x, areaOffset.x - areaSize.x / 2, areaOffset.x + areaSize.x / 2);
            newPosition.y = Mathf.Clamp(newPosition.y, areaOffset.y - areaSize.y / 2, areaOffset.y + areaSize.y / 2);
        }
       
        transform.position = newPosition;

        timer += Time.deltaTime;
        if (timer >= changeDirectionTime) {
            SetRandomDirection();
            timer = 0f;
        }
    }

    public void SetGoToPosition(Vector3 position) {
        bGoToTargetDestination = true;
        targetDestination = position;
        moveSpeed = matingSpeed;
    }

    private void PathToTarget() {
        Vector3 direction = targetDestination - transform.position;
        float distance = direction.magnitude;

        if (distance < stoppingDistance) {
            transform.position = targetDestination;
            bAtTarget = true;
            return;
        }

        direction = direction.normalized;        
        Vector3 newPosition = transform.position + direction * moveSpeed * Time.deltaTime;
        transform.position = newPosition;
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

    public void Scatter() {
        bScatter = true;
        moveSpeed = scatterSpeed;
    }

    private void AvoidPosition(Vector3 targetPosition, float avoidanceDistance) {
        Vector3 directionAwayFromTarget = transform.position - targetPosition;

        float distanceToTarget = directionAwayFromTarget.magnitude;

        if (distanceToTarget < avoidanceDistance) {
            float avoidanceFactor = Mathf.Lerp(0f, targetAvoidanceStrength, 1f - (distanceToTarget / avoidanceDistance));
            targetDirection += new Vector2(directionAwayFromTarget.x, directionAwayFromTarget.y).normalized * avoidanceFactor;
        }
    }

    public void SetMovement(bool bShouldWander) {
        bWander = bShouldWander;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(areaOffset, new Vector3(areaSize.x, areaSize.y, 0));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(currentDirection * predictionDistance));
    }
}

