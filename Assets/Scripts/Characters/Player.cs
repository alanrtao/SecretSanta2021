using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MassedMonoBehaviour
{
    [Range(0, 100)]
    public float gravity;
    [Range(0, 10)]
    public float velocity;

    public Vector2 target;

    public static Player instance
    {
        get { return _instance; }
    }

    BoxCollider bc;

    private static Player _instance;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>(); ;
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // move towards target
        float tDist = (map_pos - target).magnitude;
        float step = Mathf.Min(tDist, dt * velocity);
        map_pos += (target - map_pos).normalized * step;

        transform.position = Vehicle.Instance.Board.MapXYToWorld(map_pos) + transform.up * bc.size.y / 2;

// print(map_pos + " ~ " + Vehicle.Instance.Board.GetXY(transform.position).ToString("F3") + " -> " + target.ToString("F3"));
    }

    // Update is called once per frame
    void Update()
    {
        // rClick or rDrag
        if (Input.GetMouseButton(1) || Input.GetMouseButtonDown(1))
        {
            target = Vehicle.Instance.Board.MouseXY;
            // print(target);
            Vehicle.Instance.mouse_marker.Emit();
        }

    }
}
