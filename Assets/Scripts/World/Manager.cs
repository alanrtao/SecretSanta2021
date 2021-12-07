using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    public static Manager Instance
    {
        get { return _instance; }
    }
    private static Manager _instance;

    public float Radius;

    List<MapPoint> map;

    public GameObject TreePrototype;
    public Transform Globe;
    public Vehicle vehicle;

    // Start is called before the first frame update
    void Start()
    {
        _instance = this;

        // generate mesh
        map = new List<MapPoint>();

        GenerateMap();

        Globe.localScale = Vector3.one * Radius * 2;

        vehicle = Vehicle.Instance;
        vehicle.transform.position = vehicle.transform.position.normalized * (Radius + 5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // references Red Blob Games' tutorial, which in turn references Fil (visionscarto.net) from Observablehq.com
    // generates more-or-less equidistant points on a sphere
    void GenerateMap()
    {
        int n = 5000;

        float lonstep = 720f / (Mathf.Sqrt(5) + 1);
        float polegap = .7012f;
        float latstep = 2f / (n - 1 + 2 * polegap);
        float latstart = -1 + polegap * latstep;

        for (int i = 0; i < n; i++)
        {
            if (Random.value < 0.5f) continue;

            float lon = lonstep * i - 360 * Mathf.Round((lonstep * i) / 360f);
            float lat = Mathf.Asin(latstart + i * latstep);

            lon = Mathf.Deg2Rad * lon;

            MapPoint mp = new MapPoint(
                new Vector3(
                    Radius * Mathf.Cos(lat) * Mathf.Cos(lon),
                    Radius * Mathf.Cos(lat) * Mathf.Sin(lon),
                    Radius * Mathf.Sin(lat)
                    )
                );

            map.Add(mp);
        }

    }
}

public class MapPoint : Object
{
    public static int nNeighbors = 3;

    public Vector3 pos;

    GameObject tree;

    public MapPoint(Vector3 pos)
    {
        this.pos = pos;
        tree = Instantiate(Manager.Instance.TreePrototype, Manager.Instance.transform);
        tree.transform.position = pos;
        Vector3 ax = Vector3.Cross(tree.transform.up, pos.normalized);
        tree.transform.rotation = Quaternion.AngleAxis(
            Vector3.SignedAngle(tree.transform.up, pos.normalized, ax),
            ax
            );
    }
}
