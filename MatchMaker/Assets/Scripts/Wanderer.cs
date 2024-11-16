using UnityEngine;

public class Wanderer : MonoBehaviour {
    [SerializeField] private float moveSpeed = 2f;              
    [SerializeField] private float turnSpeed = 2f;             
    [SerializeField] private float changeDirectionTime = 2f;
    
    [SerializeField] private float predictionDistance = 0.5f;
    [SerializeField] private float wallAvoidanceStrength = 1f;

    private Vector2 currentDirection;
    private Vector2 targetDirection;
    private float timer;

    void Start() {
        SetRandomDirection();
        currentDirection = targetDirection;
    }

    void Update() {
        PredictAndAdjustDirection();

        currentDirection = Vector2.Lerp(currentDirection, targetDirection, Time.deltaTime * turnSpeed).normalized;

        Vector3 newPosition = transform.position + (Vector3)(currentDirection * moveSpeed * Time.deltaTime);

        newPosition.x = Mathf.Clamp(newPosition.x, -GameManager.Instance.areaSize.x / 2, GameManager.Instance.areaSize.x / 2);
        newPosition.y = Mathf.Clamp(newPosition.y, -GameManager.Instance.areaSize.y / 2, GameManager.Instance.areaSize.y / 2);
        transform.position = newPosition;

        timer += Time.deltaTime;
        if (timer >= changeDirectionTime) {
            SetRandomDirection();
            timer = 0f;
        }

        //RotateToFaceDirection();
    }

    private void SetRandomDirection() {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        targetDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    private void PredictAndAdjustDirection() {
        Vector3 predictedPosition = transform.position + (Vector3)(currentDirection * predictionDistance);

        if (predictedPosition.x < -GameManager.Instance.areaSize.x / 2 || predictedPosition.x > GameManager.Instance.areaSize.x / 2) {
            float adjustment = wallAvoidanceStrength * (predictedPosition.x < 0 ? 1 : -1);
            targetDirection += new Vector2(adjustment, 0).normalized;
        }

        if (predictedPosition.y < -GameManager.Instance.areaSize.y / 2 || predictedPosition.y > GameManager.Instance.areaSize.y / 2) {
            float adjustment = wallAvoidanceStrength * (predictedPosition.y < 0 ? 1 : -1);
            targetDirection += new Vector2(0, adjustment).normalized;
        }

        targetDirection = targetDirection.normalized;
    }

    private void RotateToFaceDirection() {
        if (currentDirection.sqrMagnitude > 0.01f) {
            float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        }
    }

    private void OnDrawGizmosSelected() {
        // area
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(GameManager.Instance.areaSize.x, GameManager.Instance.areaSize.y, 0));

        // forward prediction
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(currentDirection * predictionDistance));
    }
}
