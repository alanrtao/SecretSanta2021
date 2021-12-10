using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Globe : MonoBehaviour
{
    public float period; // period of one revolution

    public float Radius;
    public int SpawnCalls;
    [Range(0, 1)]
    public float SpawnRate;

    List<MapPoint> map;

    public Collider bound;

    // Start is called before the first frame update
    void Start()
    {
        // generate mesh
        map = new List<MapPoint>();

        transform.localScale = Vector3.one * Radius;

        bound = GetComponent<SphereCollider>();

        GenerateMap();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.AngleAxis(360 * Time.fixedDeltaTime / period, Vector3.up) * transform.rotation;
    }

    // references Red Blob Games' tutorial, which in turn references Fil (visionscarto.net) from Observablehq.com
    // generates more-or-less equidistant points on a sphere
    void GenerateMap()
    {
        if (!Application.isPlaying) return;

        int n = SpawnCalls;

        float lonstep = 720f / (Mathf.Sqrt(5) + 1);
        float polegap = .7012f;
        float latstep = 2f / (n - 1 + 2 * polegap);
        float latstart = -1 + polegap * latstep;

        for (int i = 0; i < n; i++)
        {
            if (Random.value > SpawnRate) continue;

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

    public class MapPoint : Object
    {
        public static int nNeighbors = 3;

        public Vector3 pos;

        GameObject tree;

        public MapPoint(Vector3 pos)
        {
            this.pos = pos;
            tree = Instantiate(Manager.Instance.TreePrototype, Manager.Instance.Globe.transform);
            tree.transform.position = pos;
            Vector3 ax = Vector3.Cross(tree.transform.up, pos.normalized);
            tree.transform.rotation = Quaternion.AngleAxis(
                Vector3.SignedAngle(Vector3.up, pos.normalized, ax),
                ax
                );
            tree.transform.localScale = Vector3.one / Manager.Instance.Globe.Radius;
        }
    }


}
