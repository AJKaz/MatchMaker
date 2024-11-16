using UnityEngine;

[RequireComponent(typeof(Boid))]
public class Agent : MonoBehaviour
{
    [SerializeField] private Boid boid;

    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float maxForce = 5f;

    private Vector3 totalForce = Vector3.zero;

    private float wanderAngle = 0f;
    [SerializeField] private float maxWanderAngle = 45f;
    [SerializeField] private float maxWanderChangePerSecond = 10f;
    [SerializeField] private float wanderWeight = 1f;

    [SerializeField] [Min(1f)] private float stayInBoundsWeight = 3f;

    private Vector3 cameraBounds;

    private void Awake()
    {
        if (boid == null) boid = GetComponent<Boid>();

        cameraBounds.y = Camera.main.orthographicSize;
        cameraBounds.x = cameraBounds.y * Camera.main.aspect;
        cameraBounds.x -= 0.25f;
        cameraBounds.y -= 0.25f;
    }

    private void Update()
    {
        CalculateSteeringForces();

        totalForce = Vector3.ClampMagnitude(totalForce, maxForce);
        boid.ApplyForce(totalForce);

        totalForce = Vector3.zero;
    }

    private void CalculateSteeringForces() {
        Wander(wanderWeight);

        StayInBounds(stayInBoundsWeight);
    }

    private void Seek(Vector3 targetPos, float weight = 1f) {
        // Calculates desired velocity
        Vector3 desiredVelocity = targetPos - boid.Position;

        // Sets desired velocity magnitude to max speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculates the seek steering force
        Vector3 seekingForce = desiredVelocity - boid.Velocity;

        // Applies force
        totalForce += seekingForce * weight;

    }

    private void Wander(float weight = 1f) {
        // Update angle of current wander
        float maxWanderChange = maxWanderChangePerSecond * Time.deltaTime;
        wanderAngle += Random.Range(-maxWanderChange, maxWanderChange);
        wanderAngle = Mathf.Clamp(wanderAngle, -maxWanderAngle, maxWanderAngle);

        // Get position that is defined by that wander angle
        Vector3 wanderTarget = Quaternion.Euler(0, 0, wanderAngle) * boid.Direction.normalized + boid.Position;

        // Seek towards wander position
        Seek(wanderTarget, weight);
    }

    protected void StayInBounds(float weight = 1f) {
        Vector3 futurePosition = GetFuturePosition();
        if (futurePosition.x > cameraBounds.x ||
            futurePosition.x < -cameraBounds.x ||
            futurePosition.y > cameraBounds.y ||
            futurePosition.y < -cameraBounds.y) {
            Seek(Vector3.zero, weight);
        }
    }

    private Vector3 GetFuturePosition(float timeToLookAhead = 1f) {
        return boid.Position + boid.Velocity * timeToLookAhead;
    }

    /*private void Evade(Agent other, float timeToLookAhead = 1f, float weight = 1f) {

        Vector3 otherFuturePosition = other.GetFuturePosition(timeToLookAhead);

        float futurePositionDistance = Vector3.SqrMagnitude(otherFuturePosition - other.boid.Position);
        float distToOther = Vector3.SqrMagnitude(boid.Position - other.boid.Position);

        if (futurePositionDistance < distToOther) {
            Flee(otherFuturePosition, weight);
        }
        else {
            Flee(other, weight);
        }

    }

    private void Flee(Agent agent, float weight = 1f) {
        Flee(agent.boid.Position, weight);
    }

    private void Flee(Vector3 targetPos, float weight = 1f) {
        // Calculate desired velocity
        Vector3 desiredVelocity = boid.Position - targetPos;

        // Sets desired velocity magnitude to max speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        // Calculates the flee steering force
        Vector3 fleeForce = desiredVelocity - boid.Velocity;

        totalForce += fleeForce * weight;
    }*/

}
