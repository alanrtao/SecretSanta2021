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

    public Material post_processing_outline;

    Camera cam;

    [SerializeField] private GameObject picture;
    [SerializeField] private Material picture_mat;

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

        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    public float t;
    void Update()
    {
        Cinemachine3rdPersonFollow vcFollow = vc.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        // tint based on location: -1 (complete back side) is midnight, 1 (complete sun) is noon

         t = Vector3.Dot(
            Manager.Instance.Globe.transform.worldToLocalMatrix.MultiplyPoint(
                Manager.Instance.Sun.transform.position
                ).normalized,
            Manager.Instance.Globe.transform.worldToLocalMatrix.MultiplyPoint(
                Vehicle.Instance.transform.position
                ).normalized
            );
        t = (1 - t) / 2;

        // print(t);

        ColorAdjustments col_adj;
        cvs.m_Profile.TryGet(out col_adj);
        Color c = filter.Evaluate(t);

        if (col_adj != null)
        {
            col_adj.colorFilter.value = c;
        }

        float cH, cS, cV;
        Color.RGBToHSV(c, out cH, out _, out cV);
        cS = Mathf.Lerp(0.5f, 0.1f, t); // more saturated during the day
        cV = Mathf.Lerp(cV, 0, Mathf.Lerp(0.45f, 1f, 1f - t)); // make less bright during the day
        c = Color.HSVToRGB(cH, cS, cV);

        post_processing_outline.SetColor("_Fill", c);

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

        // right mouse button signifies view adjustment
        if (Input.GetMouseButton(1))
        {
            Quaternion rotZ = Quaternion.AngleAxis(mouse.x, Vector3.up);
            Quaternion rotH = Quaternion.AngleAxis(mouse.y, Vector3.Cross(Vector3.up, so));

            Vector3 rawSO = rotZ * rotH * so;
            rawSO.y = Mathf.Clamp(rawSO.y, 0.5f, float.PositiveInfinity); // prevent panty shot
            vcFollow.ShoulderOffset = rawSO;
        }
        else
        {
            // when not adjusting view, camera is allowed
            if (!picture.activeInHierarchy && Input.GetMouseButtonDown(0))
            {
                StartCoroutine(PrintPicture());
            }
        }

        vc_to = -vcFollow.ShoulderOffset.normalized;

        // update actual physical parameters by lerping towards equilibrium
        vc_offset_temp = Mathf.Lerp(vc_offset_eq, vc_offset_temp, vc_smoothness);

        vcFollow.ShoulderOffset = vc_offset_temp * -vc_to;
    }

    public FMODUnity.EventReference polaroid;
    IEnumerator PrintPicture()
    {

        print("taking a picture...");
        // capture picture
        RenderTexture picture_rt = cam.targetTexture;
        RenderTexture stashed = RenderTexture.active;
        RenderTexture.active = picture_rt;

        Texture2D pic = new Texture2D(picture_rt.width, picture_rt.height);
        pic.ReadPixels(new Rect(0, 0, pic.width, pic.height), 0, 0);
        pic.Apply();

        RenderTexture.active = stashed;

        UnityEngine.UI.Image img = picture.GetComponent<UnityEngine.UI.Image>();
        img.sprite = Sprite.Create(pic, new Rect(0, 0, pic.width, pic.height), Vector2.one * .5f);

        float t = -1;
        picture_mat.mainTexture = pic;

        float f = 10;

        FMOD.Studio.EventInstance polaroid_inst = FMODUnity.RuntimeManager.CreateInstance(polaroid);
        polaroid_inst.start();
        yield return new WaitForSeconds(0.2f);

        picture.SetActive(true);
        bool polaroid_playing = true;

        RectTransform rect = picture.transform as RectTransform;
        Vector2 stg = rect.anchoredPosition;
        picture_mat.SetVector("_root", new Vector2(Random.value * 10000, Random.value * 10000));

        while (t < 0)
        {
            t += Mathf.PerlinNoise(Time.time * 1f, 0) * Time.fixedDeltaTime;
            t = Mathf.Min(t, 0);
            picture_mat.SetFloat("_t", t);

            rect.anchoredPosition =
                stg
                + 7f * new Vector2(Mathf.PerlinNoise(0, Time.time * f), Mathf.PerlinNoise(Time.time * f, 0));

            if (t > -0.2f && polaroid_playing)
            {
                polaroid_playing = false;
                polaroid_inst.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            yield return null;
        }
        rect.anchoredPosition = stg;

        yield return new WaitForSeconds(3);

        Collage.Instance.AddPicture(pic);

        Customer.Instance.CheckCriterion();

        picture.SetActive(false);
    }
}
