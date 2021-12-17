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
    }

    void Init()
    {
        q = quests[idx];

        Vector2 valid_point = Vehicle.Instance.Board.GetValidPoint();
        display.localPosition = new Vector3(valid_point.x, q.size.y, valid_point.y);
        bound.size = new Vector3(1, 2, 1);
        display.localScale = new Vector3(q.size.x, q.size.x, q.size.x);

        // update trigger position to random sphere point
        float phi = Random.value * 2 * Mathf.PI;
        float theta = Random.value * 2 * Mathf.PI;
        float r = (1.1f + Random.value * 0.2f) * Manager.Instance.Globe.Radius;

        trigger.transform.position = new Vector3(
            r * Mathf.Cos(theta) * Mathf.Cos(phi),
            r * Mathf.Cos(theta) * Mathf.Sin(phi),
            r * Mathf.Sin(theta)
            );
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(picked_up) { picked_up = false; }
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
            float h = bound.bounds.extents.y;
            float r = bound.bounds.extents.x / 2;
            display.localPosition = Player.instance.transform.position + Player.instance.transform.forward * r + new Vector3(0, h, 0);
        }
    }

    
}