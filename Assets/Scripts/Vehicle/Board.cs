using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Vector3 MouseXYZ
    {
        get { return _mouseXYZ; }
    }

    public Vector2 MouseXY
    {
        get { return _mouseXY; }
    }

    private Vector3 _mouseXYZ;
    private Vector2 _mouseXY;

    private Plane plane;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        plane = new Plane(transform.up, transform.position);
        UpdateMouse();
    }

    private void FixedUpdate()
    {
    }

    private void UpdateMouse()
    {
        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(mRay.origin, mRay.direction * 100000, Color.yellow);

        float d;
        if (plane.Raycast(mRay, out d))
        {
            _mouseXYZ = mRay.origin + mRay.direction * d;
            Vector3 localXYZ = transform.worldToLocalMatrix.MultiplyPoint(_mouseXYZ);
            _mouseXY = new Vector2(localXYZ.x, localXYZ.z);
        }
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
