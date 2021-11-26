using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{

    public float v_tan, v_norm;

    [SerializeField] private Board _board;
    public Board Board { get { return _board; } }

    public float altitude;

    public static Vehicle Instance { get { return _instance; } }
    private static Vehicle _instance;

    // marker for the mouse on the plane of the board, purely for marking purpose
    public Transform mouse_marker;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        mouse_marker.position = Board.GetMouseXYZ();
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // distribution information

        // orientational information
        Vector3 radial = transform.position.normalized;
        Vector3 f = transform.forward.normalized;
        Vector3 fR = Vector3.Dot(radial, f) * radial;
        Vector3 fT = f - fR;

        Vector3 boardRot = _board.transform.localRotation.eulerAngles;

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
        float climb = Mathf.Sin(boardRot.x);
        transform.position += transform.position.normalized * v_norm * climb;

        altitude = r;
    }
}
