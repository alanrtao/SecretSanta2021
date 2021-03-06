using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassedMonoBehaviour : MonoBehaviour
{
    // position of the object on the map
    protected Vector2 map_pos;
    public Vector2 xy { get { return map_pos; } }
    // 

    protected Transform weight_transform;

    public Vector3 weight_contribution
    {
        get { return rb == null ? Vector3.zero : rb.mass * weight_transform.localPosition; }
    }

    protected Rigidbody rb;
}
