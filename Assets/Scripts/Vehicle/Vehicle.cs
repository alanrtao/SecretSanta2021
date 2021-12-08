using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{

    public float v_tan, v_norm, dAng_dt;
    public float pitch_lerp, roll_lerp; // interpolation from linear (equally sensitive) to pow5 (sensitive only at edge)

    [SerializeField] private Board _board;
    public Board Board { get { return _board; } }

    public static Vehicle Instance { get { return _instance; } }
    private static Vehicle _instance;

    // marker for the mouse on the plane of the board, purely for marking purpose
    public MouseIndicator mouse_marker;

    private void Awake()
    {
        _instance = this;
    }

    bool first_frame = true;

    // Start is called before the first frame update
    void Start()
    {
        first_frame = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (first_frame && Manager.Instance != null)
        {
            first_frame = false;
            transform.position = (Manager.Instance.Globe.Radius + 20f) * transform.position.normalized;
            Vector3 ax = Vector3.Cross(Vector3.up, transform.position.normalized);
            transform.rotation = Quaternion.AngleAxis(
                Vector3.SignedAngle(Vector3.up, transform.position.normalized, ax),
                ax
                );
        }

        float dt = Time.fixedDeltaTime;

        // distribution information

        // orientational information
        Vector3 radial = transform.position.normalized;
        Vector3 f = transform.forward.normalized;
        Vector3 fR = Vector3.Dot(radial, f) * radial;
        Vector3 fT = f - fR;

        // positional information
        float r = transform.position.magnitude;

        // angular movement
        float aDelta = v_tan * dt / r;
        Vector3 axis = Vector3.Cross(radial, fT);

        // tangential movement via rotation
        Quaternion rot = Quaternion.AngleAxis(aDelta, axis);
        transform.position = rot * transform.position;
        transform.rotation = rot * transform.rotation;

        // climb after tangential movement
        float climb = -Mathf.Lerp(Board.mass_distrib.y, Mathf.Pow(Board.mass_distrib.y, 5), pitch_lerp);
        transform.position += 
            radial * v_norm * 
            (transform.position.magnitude > Manager.Instance.Globe.Radius + 5 ? climb : Mathf.Max(0, climb)) // when too low, only climb
            * dt;

        // turning by roll
        float roll = Mathf.Lerp(Board.mass_distrib.x, Mathf.Pow(Board.mass_distrib.x, 5), roll_lerp);
        Quaternion turn = Quaternion.AngleAxis(
            dAng_dt * dt * roll,
            radial
            );
        transform.rotation = turn * transform.rotation;
    }
}
