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

    [SerializeField] private Transform display;

    [SerializeField] private NPCTrigger trigger;

    [SerializeField] private GameObject UICanvas;

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
        display.localPosition = new Vector3(valid_point.x, q.size.y, valid_point.y);
        display.localScale = new Vector3(q.size.x, q.size.x, q.size.x);

        bound = gameObject.AddComponent<BoxCollider>();

        // update trigger position to random sphere point
        float phi = Random.value * 2 * Mathf.PI;
        float theta = Random.value * 2 * Mathf.PI;
        float r = (1.1f + Random.value * 0.2f) * Manager.Instance.Globe.Radius;

        trigger.transform.position = new Vector3(
            r * Mathf.Cos(theta) * Mathf.Cos(phi),
            r * Mathf.Cos(theta) * Mathf.Sin(phi),
            r * Mathf.Sin(theta)
            );

        weight_transform = display;
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

                Vector3 t = display.localPosition;
                t.y = bound.size.y * display.localScale.y / 2;
                display.localPosition = t;
            }
            else
            {
                if ((display.position - Player.instance.transform.position).magnitude < 1) {
                    picked_up = true;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (picked_up)
        {
            float h = bound.size.y * display.localScale.y;
            float r = bound.size.x * display.localScale.x;
            print(Player.instance.dir);
            Vector3 xy = Player.instance.transform.localPosition;
            map_pos = new Vector2(xy.x, xy.z) + Player.instance.dir * r;
            map_pos = CustomMaths.Clamp(map_pos, Vehicle.Instance.Board.size / 2);
            display.localPosition = new Vector3(map_pos.x, h, map_pos.y);
        } else
        {
        }
    }

    
}