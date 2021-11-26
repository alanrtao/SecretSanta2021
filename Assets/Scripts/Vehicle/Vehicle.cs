using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{

    public float v_tan, v_norm;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        // orientational information
        Vector3 radial = transform.position.normalized;
        Vector3 f = transform.forward.normalized;
        Vector3 fR = Vector3.Dot(radial, f) * radial;
        Vector3 fT = f - fR;

        // positional information
        float r = transform.position.magnitude;

        // angular movement
        float aDelta =v_tan * dt / r;
        Vector3 axis = Vector3.Cross(fT, radial);

        // tangential movement via rotation
        Quaternion rot = Quaternion.AngleAxis(aDelta, axis);
        transform.position = rot * transform.position;
        transform.rotation = rot * transform.rotation;

        // climb after tangential movement
        transform.position += transform.position.normalized * v_norm;
    }
}
