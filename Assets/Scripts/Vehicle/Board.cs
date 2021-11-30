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

    [SerializeField] private float w, h;
    [SerializeField] private Transform display; // the board to display on screen

    public Vector2 size { get { return new Vector2(w, h); } }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        plane = new Plane(transform.up, transform.position);
        UpdateMouse();

        display.localScale = new Vector3(w / 10, 1, h / 10);
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
            if (Mathf.Abs(_mouseXY.x) > w/2f)
            {
                float ratio = (w / 2f) / Mathf.Abs(_mouseXY.x);
                _mouseXY *= ratio;
            }
            if (Mathf.Abs(_mouseXY.y) > h / 2f)
            {
                float ratio = (h / 2f) / Mathf.Abs(_mouseXY.y);
                _mouseXY *= ratio;
            }
            _mouseXYZ = transform.localToWorldMatrix.MultiplyPoint(new Vector3(_mouseXY.x, 0, _mouseXY.y));

            Debug.DrawLine(transform.localToWorldMatrix.MultiplyPoint(new Vector3(-w / 2f, 0, h / 2f)), transform.localToWorldMatrix.MultiplyPoint(new Vector3(w / 2f, 0, h / 2f)));
            Debug.DrawLine(transform.localToWorldMatrix.MultiplyPoint(new Vector3(-w / 2f, 0, -h / 2f)), transform.localToWorldMatrix.MultiplyPoint(new Vector3(-w / 2f, 0, h / 2f)));
            Debug.DrawLine(transform.localToWorldMatrix.MultiplyPoint(new Vector3(w / 2f, 0, -h / 2f)), transform.localToWorldMatrix.MultiplyPoint(new Vector3(w / 2f, 0, h / 2f)));
            Debug.DrawLine(transform.localToWorldMatrix.MultiplyPoint(new Vector3(-w / 2f, 0, -h / 2f)), transform.localToWorldMatrix.MultiplyPoint(new Vector3(w / 2f, 0, -h / 2f)));
        }
    }

    // convert world absolute position to xy position on the board
    public Vector2 GetXY(Vector3 worldpos)
    {
        Vector3 localXYZ = transform.worldToLocalMatrix.MultiplyPoint(worldpos);
        return new Vector2(localXYZ.x, localXYZ.z); ;
    }

    // convert world absolute position to uv position on the board, for uv from [-1, 1]
    public Vector2 GetUV(Vector3 worldpos)
    {

        return GetXY(worldpos) / 5;
    }

    public Vector3 UVToWorld(Vector2 uv)
    {
        return transform.position + transform.forward * uv.y * transform.localScale.z + transform.right * uv.x * transform.localScale.x;
    }

    public Vector3 MapXYToWorld(Vector2 map_pos)
    {
        Vector2 uv = new Vector2(map_pos.x / transform.localScale.x, map_pos.y / transform.localScale.y);
        return UVToWorld(uv);
    }

    // convert worldpos to uv position on the board
    public float GetRadialNormalized(Vector3 worldpos)
    {
        return GetUV(worldpos).x;
    }
}
