using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MassedMonoBehaviour
{
    [Range(0, 100)]
    public float gravity;

    public Vector2 target;

    public static Player instance
    {
        get { return _instance; }
    }

    private static Player _instance;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // apply gravity
        rb.AddForce(gravity * -transform.position.normalized * rb.mass, ForceMode.Force);
    }

    // Update is called once per frame
    void Update()
    {
        // rClick or rDrag
        if(Input.GetMouseButton(1) || Input.GetMouseButtonDown(1))
        {
            target = Vehicle.Instance.Board.MouseXY;
        }   
    }
}
