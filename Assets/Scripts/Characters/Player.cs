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
        bc = GetComponent<BoxCollider>();

        weight_transform = transform;

        Vehicle.Instance.Board.masses.Add(this);
    }

    public Vector2 dir = Vector2.zero;

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        Vector2 axis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (axis.magnitude > 1)
        {
            axis = axis.normalized;
        }
        map_pos += axis * dt * velocity;
        map_pos = CustomMaths.Clamp(map_pos, Vehicle.Instance.Board.size/2);

        if (axis.magnitude > 0) { dir = new Vector2(axis.x, axis.y).normalized; }
        transform.localPosition = new Vector3(map_pos.x, bc.size.y * transform.localScale.y / 2, map_pos.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Manager.Instance.Globe.Achievement(transform.position);
        }
        // rClick or rDrag
        /*if (Input.GetMouseButton(1) || Input.GetMouseButtonDown(1))
        {
            target = Vehicle.Instance.Board.MouseXY;
            // print(target);
            Vehicle.Instance.mouse_marker.Emit();
        }*/

    }
}
