using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float elevation;
    public Transform display;

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
        // determine position based on relative position from the vehicle and the camera view
        RaycastHit a2b_hit;
        Camera c = Manager.Instance.mCam;
        Vector3 to = (transform.position - c.transform.position);
        float dist = to.magnitude;
        to /= dist;

        Debug.DrawLine(transform.position, c.transform.position, Color.magenta);

        if (
            Manager.Instance.Globe.bound.Raycast(
                new Ray(c.transform.position, to),
                out a2b_hit,
                dist
                )
            )
        {
            // print(rhit);
            // on backside of sphere, go to the closest point on the horizon
            Vector3 hit = a2b_hit.point;
            Quaternion perp = Quaternion.AngleAxis(90, Vector3.Cross(to, hit.normalized));
            Vector3 perp_vec = (Manager.Instance.Globe.Radius + elevation * Mathf.Sin(Time.frameCount / 120f)) * (perp * to);
            display.position = perp_vec;
        }
        else
        {
            // not on backside of sphere, just go to normal position
            display.localPosition = Vector3.zero;
        }

        display.localScale = Vector3.one * dist;
        display.forward = transform.position - c.transform.position;
    }
}
