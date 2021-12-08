using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassedMonoBehaviour : MonoBehaviour
{
    // position of the object on the map
    protected Vector2 map_pos;

    // 

    public Vector3 weight_contribution
    {
        get { return (rb == null ? Vector3.zero : rb.mass * transform.localPosition); }
    }

    protected Rigidbody rb;
}
