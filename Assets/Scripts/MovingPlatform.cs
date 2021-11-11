using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{

    [Header("Platform Settings")]
    [SerializeField] bool shouldRotate;
    [SerializeField] bool shouldMove;
   
    [Header("Rotation")]
    [SerializeField] float rotateSpeed;
    [SerializeField] RotationDirectionEnum DirectionToRotate;
    [SerializeField] bool useMaxRotation;
    [Range(0f, 360f)]
    [SerializeField] float maxLeftRotation;
    [Range(0f, 360f)]
    [SerializeField] float maxRightRotation;
    [SerializeField] float waitTimeLeft;
    [SerializeField] float waitTimeRight;
    float timeToRotate;
    float targetRotation;
    RotationDirectionEnum currentRotationDirection;
    bool canRotate;

    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] WaypointStruct[] Waypoints;
    [SerializeField] WaypointMovementTypeEnum WaypointMovementType;
    bool canMove;
    WaypointStruct currentWaypoint;
    float timeToMove;
    bool BounceSwitch;
    int currentWaypointIndex;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        SetTargetRotation(DirectionToRotate);
        currentRotationDirection = DirectionToRotate;
        if (!(Waypoints?.Length != 0))
        {
            shouldMove = false;
        }
        else
        {
            currentWaypoint = Waypoints[0];
            currentWaypointIndex = 0;
        }
        if (shouldRotate) canRotate = true;
        if (shouldMove) canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove) canMove = CheckTimes(timeToMove);
        if (shouldRotate) canRotate = CheckTimes(timeToRotate);

        if (canRotate)
        {
            if (useMaxRotation) CheckRotation();
            Rotate(currentRotationDirection);
        }

        CheckDistanceToWaypoint();
        if (canMove)
        {
            Move();
        }
    }

    void Move()
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = currentWaypoint.position;
        Vector2 dir = (currentPos - targetPos).normalized;
        rb.velocity = -(dir * movementSpeed);
    }

    void CheckDistanceToWaypoint()
    {
        if (Vector3.Distance(transform.position, currentWaypoint.position) < 0.01f)
        {
            timeToMove = Time.time + currentWaypoint.waitTime;
            GetNewWaypoint();
        }
    }

    void GetNewWaypoint()
    {
        switch (WaypointMovementType)
        {
            case WaypointMovementTypeEnum.Loop:
                {
                    currentWaypointIndex = ((int)Mathf.Repeat(currentWaypointIndex + 1, Waypoints.Length));
                    currentWaypoint = Waypoints[currentWaypointIndex];
                    break;
                }
            case WaypointMovementTypeEnum.Bounce:
                {
                    int i = 0;
                    if (BounceSwitch)
                    {
                        i = currentWaypointIndex - 1;
                        if(i < 0)
                        {
                            BounceSwitch = false;
                            currentWaypointIndex += 1;
                        }
                        else
                        {
                            currentWaypointIndex = i;
                        }
                    }
                    else if (!BounceSwitch)
                    {
                        i = currentWaypointIndex + 1;
                        if (i > Waypoints.Length - 1)
                        {
                            BounceSwitch = true;
                            currentWaypointIndex -= 1;
                        }
                        else
                        {
                            currentWaypointIndex = i;
                        }
                    }
                    currentWaypoint = Waypoints[currentWaypointIndex];
                    break;
                }
            case WaypointMovementTypeEnum.Reset:
                {
                    currentWaypointIndex = ((int)Mathf.Repeat(currentWaypointIndex + 1, Waypoints.Length));
                    if(currentWaypointIndex == 0)
                    {
                        transform.position = Waypoints[0].position;
                    }
                    currentWaypoint = Waypoints[currentWaypointIndex];
                    break;
                }
            default: break;
        }
    }


    bool CheckTimes(float time)
    {
        if (Time.time > time)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void CheckRotation()
    {
        if (almostEqual(transform.rotation.eulerAngles.z, targetRotation, 1f))
        {
            switch (currentRotationDirection)
            {
                case RotationDirectionEnum.Left:
                    {
                        currentRotationDirection = RotationDirectionEnum.Right;
                        SetTargetRotation(currentRotationDirection);
                        timeToRotate = Time.time + waitTimeLeft;
                        break;
                    }
                case RotationDirectionEnum.Right:
                    {
                        currentRotationDirection = RotationDirectionEnum.Left;
                        SetTargetRotation(currentRotationDirection);
                        timeToRotate = Time.time + waitTimeRight;
                        break;
                    }
                default: break;
            }
        }
    }


    void Rotate(RotationDirectionEnum dir)
    {
        switch (dir)
        {
            case RotationDirectionEnum.Left:
                {
                    transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + rotateSpeed * Time.deltaTime);
                    break;
                }
            case RotationDirectionEnum.Right:
                {
                    transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - rotateSpeed * Time.deltaTime);
                    break;
                }
            default: break;
        }
    }

    public static bool almostEqual(float a, float b, float eps)
    {
        return Mathf.Abs(a - b) < eps;
    }


    void SetTargetRotation(RotationDirectionEnum dir)
    {
        switch (dir)
        {
            case RotationDirectionEnum.Left:
                {
                    targetRotation = maxLeftRotation;
                    break;
                }
            case RotationDirectionEnum.Right:
                {
                    targetRotation = 360f - maxRightRotation; 
                    break;
                }
            default: break;
        }
    }


    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position

        if ((Waypoints?.Length != 0))
        {
            Gizmos.color = Color.blue;
            foreach (var waypoint in Waypoints)
            {
                Gizmos.DrawSphere(waypoint.position, 0.5f);
            }
        }
    }

}

[System.Serializable]
public enum RotationDirectionEnum
{
     Left,
     Right,
}

[System.Serializable] 
public struct WaypointStruct
{
    public Vector3 position;
    public float waitTime;
}

[System.Serializable] 
public enum WaypointMovementTypeEnum
{
    Loop,
    Bounce,
    Reset,
}