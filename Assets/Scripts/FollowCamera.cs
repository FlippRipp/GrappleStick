using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] float smoothSpeed = 0.125f;
    [SerializeField] Vector3 offset;


    // moves the camera after the player position, tracking the player att the given smoothspeed, giving a smooth camera movement.
    void FixedUpdate()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}