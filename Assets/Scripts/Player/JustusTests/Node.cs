using System.Collections;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 normalDirection;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, normalDirection);
    }
}