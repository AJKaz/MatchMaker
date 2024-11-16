using UnityEngine;

public class Boid : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 acceleration;
    private Vector3 direction;

    //[SerializeField] private bool bBounceOffWalls = true;
    [SerializeField] private float mass = 1f;

    // Used for wall detection
    //private Vector3 cameraSize;

    /* Getters */
    public Vector3 Velocity => velocity;
    public Vector3 Direction => direction;
    public Vector3 Position => transform.position;
    //public Vector3 CameraSize => cameraSize;

    void Start()
    {
        // Distance from middle of camera to top & side
        /*cameraSize.y = Camera.main.orthographicSize;
        cameraSize.x = cameraSize.y * Camera.main.aspect;*/

        direction = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        // Calculate new velocity based on current acceleration of object
        velocity += acceleration * Time.deltaTime;

        // Calculate new postion based on velocity for this frame
        transform.position += velocity * Time.deltaTime;

        if (velocity.sqrMagnitude > Mathf.Epsilon) {
            // Store direction that object is moving in
            direction = velocity.normalized;
        }

        // Zero out acceleration for next frame
        acceleration = Vector3.zero;

        // Make object face direction it's moving
        // TODO: May need to update this to work wiht people sprites
        transform.rotation = Quaternion.LookRotation(Vector3.back, direction);

        /*if (bBounceOffWalls) {
            BounceOffWalls();
        }*/
    }

    public void ApplyForce(Vector3 force) {
        acceleration += force / mass;
    }

    /*private void BounceOffWalls() {
        // Keep object on screen in terms of x:
        if (transform.position.x > cameraSize.x && velocity.x > 0) {
            velocity.x *= -1f;
        }
        if (transform.position.x < -cameraSize.x && velocity.x < 0) {
            velocity.x *= -1f;
        }
        // Keep object on screen in terms of y:
        if (transform.position.y > cameraSize.y && velocity.y > 0) {
            velocity.y *= -1f;
        }
        if (transform.position.y < -cameraSize.y && velocity.y < 0) {
            velocity.y *= -1f;
        }
    }*/

}
