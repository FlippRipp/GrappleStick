using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerMovement : MonoBehaviour
{
    private Vector2[] vertices = new Vector2[64];

    private void RopeCut()
    {
        //if (grappleHook && grappleHook.gameObject.activeInHierarchy)
        //{
        //    Vector2 pos = grappleHook.transform.position;

        //    Vector2 delta = -((Vector2)pos - (Vector2)transform.position);

        //    RaycastHit2D ray = Physics2D.Raycast(pos, delta.normalized, delta.magnitude, grappleHook.grappleLayers);

        //    if (ray.collider)
        //    {
        //        CompositeCollider2D cc2d = ray.collider as CompositeCollider2D;
        //        if (cc2d)
        //        {
        //            Vector2 point = Vector2.positiveInfinity;

        //            float currentLength = Mathf.Infinity;

        //            Vector3 meanPos = (ray.point);

        //            for (int i = 0; i < cc2d.pathCount; i++)
        //            {
        //                for (int ii = 0; ii < cc2d.GetPath(i, vertices); ii++)
        //                {
        //                    float thisLength = Vector2.SqrMagnitude((Vector2)meanPos - vertices[ii]);
        //                    if (thisLength < currentLength)
        //                    {
        //                        currentLength = thisLength;
        //                        point = vertices[ii];
        //                    }
        //                }
        //            }

        //            Debug.DrawLine(grappleHook.transform.position, point, Color.red);
        //            Debug.DrawLine(point, transform.position, Color.red);
        //        }

        //    }
        //    else
        //    {
        //        Debug.DrawLine(grappleHook.transform.position, transform.position, Color.red);
        //    }
        //}

    }
}
