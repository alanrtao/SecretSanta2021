using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Customer : MassedMonoBehaviour
{
    public bool picked_up = false;

    [SerializeField] private BoxCollider bound;

    Quest q;

    [SerializeField] private Quest[] quests;
    int idx = -1;

    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private Transform center, display;

    [SerializeField] private NPCTrigger trigger;

    [SerializeField] private GameObject UICanvas;

    [SerializeField] private GameObject picture;

    private void Awake()
    {
        idx = -1; // go to first customer when scene load
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnEnable()
    {
        idx++;
        if (idx >= quests.Length) { Manager.Instance.GameEnd(); return; }
        Init();

        Vehicle.Instance.Board.masses.Add(this);

        UICanvas.SetActive(true);
    }

    private void OnDisable()
    {
        UICanvas.SetActive(false);
        Vehicle.Instance.Board.masses.RemoveAll((m) => (m == this));
    }

    void Init()
    {
        q = quests[idx];

        if (bound != null) Destroy(bound);

        Vector2 valid_point = Vehicle.Instance.Board.GetValidPoint();

        bound = center.gameObject.AddComponent<BoxCollider>();

        center.localScale = new Vector3(q.size.x, q.size.x, q.size.x);
        center.localPosition = new Vector3(valid_point.x, bound.size.y * center.localScale.y / 2, valid_point.y);

        // update trigger position to random sphere point
        float phi = Random.value * 2 * Mathf.PI;
        float theta = Random.value * 2 * Mathf.PI;
        float r = (1.1f + Random.value * 0.2f) * Manager.Instance.Globe.Radius;

        trigger.transform.position = new Vector3(
            r * Mathf.Cos(theta) * Mathf.Cos(phi),
            r * Mathf.Cos(theta) * Mathf.Sin(phi),
            r * Mathf.Sin(theta)
            );

        weight_transform = center;
        rb = GetComponent<Rigidbody>();
        rb.mass = q.size.x / .3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(picked_up) {
                picked_up = false;

                Vector3 t = center.localPosition;
                t.y = bound.size.y * center.localScale.y / 2;
                center.localPosition = t;
            }
            else
            {
                if ((center.position - Player.instance.transform.position).magnitude < 1) {
                    picked_up = true;
                }
            }
        }
    }



    private void FixedUpdate()
    {
        if (picked_up)
        {
            float h = bound.size.y * center.localScale.y;
            float r = bound.size.x * center.localScale.x;

            float theta = Mathf.PerlinNoise(0, 10 * Time.time);
            Vector3 wiggle = new Vector3(Mathf.Sin(theta), Mathf.Cos(theta)) * Mathf.PerlinNoise(10 * Time.time, 0) * .2f / display.localScale.x;

            Vector3 xy = Player.instance.transform.localPosition;
            map_pos = new Vector2(xy.x, xy.z) + Player.instance.dir * r;
            map_pos = CustomMaths.Clamp(map_pos, Vehicle.Instance.Board.size / 2);
            center.localPosition = new Vector3(map_pos.x, h, map_pos.y);
            display.localPosition = wiggle;
        } else
        {
        }
    }

    
}