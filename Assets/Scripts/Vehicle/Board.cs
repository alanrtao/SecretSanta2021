using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetMouseXYZ()
    {
        // for source s, direction r, time t, plane through p0 with normal n
        // let line l = s + rt
        // let plane = { (p-p0) dot n = 0 }
        // (s+tr-p0) dot n = 0
        // t = (p0 - s) dot n / r dot n

        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 normal = transform.up;

        float calibration = 1000; // scale up to make things more accurate

        // solve for t, and then use that to deduce mouse position on the plane
        float t = Vector3.Dot(calibration * (transform.position - mRay.origin), normal) / Vector3.Dot(calibration * mRay.direction, normal);

        return mRay.origin + mRay.direction * t;
    }

    // convert world absolute position to xy position on the board
    public Vector2 GetXY(Vector3 worldpos)
    {
        return Vector2.zero;
    }

    // convert world absolute position to uv position on the board, for uv from [-1, 1]
    public Vector2 GetUV(Vector3 worldpos)
    {
        return GetXY(worldpos);
    }

    // convert worldpos to uv position on the board
    public float GetRadialNormalized(Vector3 worldpos)
    {
        return GetUV(worldpos).x;
    }
}
