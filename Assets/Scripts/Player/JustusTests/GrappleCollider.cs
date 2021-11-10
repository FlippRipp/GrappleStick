using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleCollider : MonoBehaviour
{
    [SerializeField]
    private Collider2D _collider;

    private Collider2D[] _results = new Collider2D[32];

    private ContactFilter2D filter;

    [SerializeField]
    private LayerMask _lm;

    private void Awake()
    {
        filter.SetLayerMask(_lm);
    }

    private void Update()
    {
        for (int i = 0; i < _collider.OverlapCollider(filter, _results); i++)
        {
            ColliderDistance2D dist = _collider.Distance(_results[i]);
            
            //if (dist.isOverlapped)
                transform.position += (Vector3)dist.normal * dist.distance;
        }
    }
}
