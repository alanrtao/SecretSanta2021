using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OverheadCamera : MonoBehaviour
{

    private CinemachineVirtualCamera vc;
    private Cinemachine.PostFX.CinemachineVolumeSettings cvs; // post processing control
    [SerializeField] private Gradient filter;

    private float vc_offset_eq; // equilibrium camera distance
    private float vc_offset_temp; // current camera distance

    [Range(0, 1)]
    public float vc_smoothness;

    [Range(5, 1000)]
    public float cNear;
    [Range(5, 1000)]
    public float cFar;

    [Range(0, 30)]
    public float zoom_sensitivity;
    [Range(0, 30)]
    public float sensitivity;

    private Vector3 vc_to; // normal vector pointing from vc to lookAt
    [Range(0, 1), SerializeField] private float cam_smoothness;
    Vector3 shoulder_offset_eq = Vector3.zero;
    Quaternion rot_eq = Quaternion.identity;

    // Start is called before the first frame update
    void Start()
    {
        vc = GetComponent<CinemachineVirtualCamera>();
        cvs = GetComponent<Cinemachine.PostFX.CinemachineVolumeSettings>();

        Cinemachine3rdPersonFollow vcFollow = vc.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        vc_offset_eq = vcFollow.ShoulderOffset.magnitude;
        vc_offset_temp = vc_offset_eq;
        vc_to = -vcFollow.ShoulderOffset.normalized;

        rot_eq = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Cinemachine3rdPersonFollow vcFollow = vc.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        // tint based on location: -1 (complete back side) is midnight, 1 (complete sun) is noon

        float t = Vector3.Dot(
            Manager.Instance.Globe.transform.worldToLocalMatrix.MultiplyPoint(
                Manager.Instance.Sun.transform.position
                ).normalized,
            transform.position.normalized);
        t = (1 - t) / 2;

        ColorAdjustments col_adj;
        cvs.m_Profile.TryGet(out col_adj);
        if (col_adj != null)
        {
            col_adj.colorFilter.value = filter.Evaluate(t);
        }

        // coladj.parameters[2] = new UnityEngine.Rendering.ColorParameter(filter.Evaluate(t));

        // zooming
        float sw = Input.GetAxis("Mouse ScrollWheel");
        vc_offset_eq = Mathf.Clamp(
            vc_offset_eq - sw * zoom_sensitivity,
            cNear,
            cFar);

        Vector2 mouse = new Vector2(
            Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        mouse *= sensitivity;

        Vector3 so = vcFollow.ShoulderOffset;

        // middle mouse button signifies view adjustment
        if (Input.GetMouseButton(2))
        {
            Quaternion rotZ = Quaternion.AngleAxis(mouse.x, Vector3.up);
            Quaternion rotH = Quaternion.AngleAxis(mouse.y, Vector3.Cross(Vector3.up, so));

            Vector3 rawSO = rotZ * rotH * so;
            rawSO.y = Mathf.Clamp(rawSO.y, 0.5f, float.PositiveInfinity); // prevent panty shot
            vcFollow.ShoulderOffset = rawSO;
        } else
        {
            // 
        }

        vc_to = -vcFollow.ShoulderOffset.normalized;

        // update actual physical parameters by lerping towards equilibrium
        vc_offset_temp = Mathf.Lerp(vc_offset_eq, vc_offset_temp, vc_smoothness);

        vcFollow.ShoulderOffset = vc_offset_temp * -vc_to;
    }
}
