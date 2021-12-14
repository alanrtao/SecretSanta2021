using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Globe : MonoBehaviour
{
    public float period; // period of one revolution

    public List<MapPoint> map;
    public List<Tree> trees;

    public System.Tuple<MapPoint, Tree> Access (int i) { return new System.Tuple<MapPoint, Tree>(map[i], trees[i]); }

    public Collider bound;

    public float Radius;

    public float season;

    public Texture[] season_skins;

    public Material tree_skin, cloud_skin;

    // Start is called before the first frame update
    void Start()
    {
        // generate mesh
        map = new List<MapPoint>();

        transform.localScale = Vector3.one * Radius;

        bound = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    float season_counter = 0;
    int season_id = 0;
    void Update()
    {
        season_counter += Time.deltaTime;
        if (season_counter >= season)
        {
            season_counter = 0;
            season_id = (season_id + 1) % 4;
            tree_skin.mainTexture = season_skins[season_id];
        }
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.AngleAxis(360 * Time.fixedDeltaTime / period, Vector3.up) * transform.rotation;

        float t = cloud_skin.GetFloat("_t") + Time.fixedDeltaTime / 300f;
        cloud_skin.SetFloat("_t", t);
        cloud_skin.SetFloat("_cloudiness", Mathf.PerlinNoise(0, t * 6) * 0.15f);
    }

    public void Spawn()
    {
        trees = new List<Tree>();
        foreach(MapPoint p in map)
        {
            trees.Add(Instantiate(p.index));
        }
    }

    // turn the nearest tree from p to golden!
    public void Achievement(Vector3 p)
    {
        MapPoint.Nearest order = new MapPoint.Nearest(p);
        MapPoint min = MapPoint.nullPoint;

        map.ForEach(i => i.Flush());

        for (int i = 0; i < map.Count; i++)
        {
            if (trees[i].accomplished == false)
            {
                min = map[i];
                break;
            }
        }
        if (min == MapPoint.nullPoint) return;

        for (int i = min.index + 1; i < map.Count; i++)
        {
            if (order.Compare(min, map[i]) > 0 && !trees[i].accomplished)
            {
                min = map[i];
            }
        }
        trees[min.index].accomplished = true;
    }

    [SerializeField] private GameObject TreePrototype;
    public Tree Instantiate(int i)
    {
        GameObject go = GameObject.Instantiate(TreePrototype, Manager.Instance.Globe.transform);
        // print(Radius);
        go.transform.position = map[i].map_pos * Radius;
        Tree tree = go.GetComponent<Tree>();
        tree.transform.up = tree.transform.position.normalized;
        tree.transform.localScale = Vector3.one / Manager.Instance.Globe.Radius;

        return tree;
    }
}

[System.Serializable]
public class MapPoint
{
    public static int nNeighbors = 3;

    public Vector3 map_pos;

    Vector3 wpos;
    public Vector3 pos
    {
        get { return wpos; }
    }

    public int index;

    // public List<MapPoint> neighbor;

    public Nearest nearest;

    public List<int> neighbors;

    Vector2 latlon;

    public static MapPoint nullPoint = new MapPoint(1, 1, -1);

    public MapPoint(float lat, float lon, int index)
    {
        this.index = index;

        latlon = new Vector2(lat, lon);

        map_pos = new Vector3(
                    Mathf.Cos(lat) * Mathf.Cos(lon),
                    Mathf.Cos(lat) * Mathf.Sin(lon),
                    Mathf.Sin(lat)
                    );

        nearest = new Nearest(this);
    }

    // update world position to fit the current globe orientation
    public void Flush()
    {
        wpos = Manager.Instance.Globe.transform.localToWorldMatrix.MultiplyPoint(map_pos);
    }

    public float Distance(MapPoint mp)
    {
        return (mp.pos - pos).magnitude;
    }

    public static System.Converter<MapPoint, int> ext = (m) => m == null ? -1 : m.index;

    public class Nearest: Comparer<MapPoint>
    {
        Vector3 pos;
        public Nearest (MapPoint anchor)
        {
            pos = anchor.pos;
        }

        public Nearest (Vector3 pos)
        {
            this.pos = pos;
        }

        public override int Compare(MapPoint a, MapPoint b)
        {
            return Mathf.RoundToInt(Mathf.Sign(
                (a.pos - pos).sqrMagnitude - (b.pos - pos).sqrMagnitude
                ));
        }
    }

    public class NearestLatLon: Comparer<MapPoint>
    {
        Vector2 latlon;
        public NearestLatLon (Vector2 latlon)
        {
            this.latlon = latlon;
        }
        public override int Compare(MapPoint a, MapPoint b)
        {
            return Mathf.RoundToInt(Mathf.Sign(
                (a.latlon - latlon).sqrMagnitude - (b.latlon - latlon).sqrMagnitude
                ));
        }
    }
}