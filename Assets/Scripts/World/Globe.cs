using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Globe : MonoBehaviour
{
    public float period; // period of one revolution

    public List<MapPoint> map;

    public Collider bound;

    public float Radius;

    public float season;

    public Texture[] season_skins;

    public Material tree_skin, cloud_skin;

    public GameObject tree_prefab;

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
        foreach(MapPoint p in map)
        {
            p.Instantiate(tree_prefab);
        }
    }
}

[System.Serializable]
public class MapPoint : Object
{
    public static int nNeighbors = 3;

    public Vector3 pos;

    GameObject tree;

    public int index;

    public List<MapPoint> neighbor;

    public Nearest nearest;

    Vector2 latlon;

    public MapPoint(float lat, float lon, Vector3 pos, int index)
    {
        this.pos = pos;
        this.index = index;

        latlon = new Vector2(lat, lon);

        nearest = new Nearest(this);
    }

    public GameObject Instantiate(GameObject prototype)
    {
        if (tree != null) return tree;
        tree = Instantiate(Manager.Instance.TreePrototype, Manager.Instance.Globe.transform);
        tree.transform.position = pos;

        Vector3 ax = Vector3.Cross(tree.transform.up, pos.normalized);
        tree.transform.rotation = Quaternion.AngleAxis(
            Vector3.SignedAngle(Vector3.up, pos.normalized, ax),
            ax
            );
        tree.transform.localScale = Vector3.one / Manager.Instance.Globe.Radius;

        return tree;
    }

    public float Distance(MapPoint mp)
    {
        return (mp.pos - pos).magnitude;
    }

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