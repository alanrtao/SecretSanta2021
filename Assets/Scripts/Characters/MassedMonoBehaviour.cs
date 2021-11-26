using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassedMonoBehaviour : MonoBehaviour
{
    // position of the object on the map
    Vector2 map_pos;

    // 

    public float weight_contribution
    {
        get { return Vehicle.Instance.Board.GetRadialNormalized(rb.position)*(rb == null ? 0 : rb.mass); }
    }

    protected Rigidbody rb;
}
