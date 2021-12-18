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
    public Camera raycastCamera;

    public Vector2 size { get { return new Vector2(w, h); } }

    public List<MassedMonoBehaviour> masses = new List<MassedMonoBehaviour>();

    Vector2 center_of_mass_eq = Vector2.zero;
    Vector2 center_of_mass = Vector2.zero;

    public Vector2 mass_distrib { get { return center_of_mass; } }
    
    [Range(0, 1), SerializeField] private float com_smoothness; // interpolation factor

    [Range(0, 90), SerializeField] private float pitch_ext;
    [Range(0, 90), SerializeField] private float roll_ext;



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

    Quaternion rot_eq;
    [Range(0, 1), SerializeField] private float rotation_smoothness;
    private void FixedUpdate()
    {
        Vector3 c = Vector3.zero;
        foreach (MassedMonoBehaviour m in masses)
        {
            // print(m.name + " -> " + m.weight_contribution);
            c += m.weight_contribution;
        }
        center_of_mass_eq.x = c.x / (w / 2);
        center_of_mass_eq.y = c.z / (h / 2);
        
        rot_eq = Quaternion.Euler(
            pitch_ext * center_of_mass.y, // pitch
            0, // yaw
            - roll_ext * center_of_mass.x // roll
            );

        transform.localRotation = rot_eq;

        // print(center_of_mass);
        center_of_mass = CustomMaths.Lerp(center_of_mass, center_of_mass_eq, com_smoothness);
    }

    private void UpdateMouse()
    {
        Ray mRay = raycastCamera.ViewportPointToRay(Camera.main.ScreenToViewportPoint(Input.mousePosition));

        Debug.DrawRay(mRay.origin, mRay.direction * 100000, Color.yellow);

        float d;
        if (plane.Raycast(mRay, out d))
        {
            _mouseXYZ = mRay.origin + mRay.direction * d;
            Vector3 localXYZ = transform.worldToLocalMatrix.MultiplyPoint(_mouseXYZ);
            _mouseXY = new Vector2(localXYZ.x, localXYZ.z);
            if (Mathf.Abs(_mouseXY.x) > w / 2f)
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

    public Vector2 GetValidPoint()
    {
        for(int i = 0; i < 10; i++)
        {
            Vector2 xy = new Vector2(Random.value, Random.value);

            float dist = (xy - Player.instance.xy).magnitude;
            if (dist > 0.3f)
            {
                return xy;
            }
        }
        return Vector2.zero;
    }
}
