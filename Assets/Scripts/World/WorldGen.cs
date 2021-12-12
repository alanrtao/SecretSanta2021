using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class WorldGen : MonoBehaviour
{
    public int SpawnCalls;
    [Range(0, 1)]
    public float SpawnRate;

    List<MapPoint> map;

    public float Radius;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            Evoke();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Evoke()
    {
        Execute();
        Manager.Instance.Globe.Radius = Radius;
        Manager.Instance.Globe.map = map;
        Manager.Instance.Globe.Spawn();
    }

    public void Execute()
    {

        if (!Application.isPlaying) return;

        int n = SpawnCalls;

        float lonstep = 720f / (Mathf.Sqrt(5) + 1);
        float polegap = .7012f;
        float latstep = 2f / (n - 1 + 2 * polegap);
        float latstart = -1 + polegap * latstep;

        map = new List<MapPoint>();

        for (int i = 0; i < n; i++)
        {
            if (Random.value > SpawnRate) continue;

            float lon = lonstep * i - 360 * Mathf.Round((lonstep * i) / 360f);
            float lat = Mathf.Asin(latstart + i * latstep);

            lon = Mathf.Deg2Rad * lon;

            MapPoint mp = new MapPoint(
                lat,
                lon,
                new Vector3(
                    Radius * Mathf.Cos(lat) * Mathf.Cos(lon),
                    Radius * Mathf.Cos(lat) * Mathf.Sin(lon),
                    Radius * Mathf.Sin(lat)
                    ),
                i
                );
            map.Add(mp);
        }

        // sort distances
        for (int i = 0; i < map.Count; i++)
        {
            List<MapPoint> sorted = new List<MapPoint>(map);
            sorted.Sort(map[i].nearest);
            // sorted.RemoveAt(0); // pop nearest (first)

            // add indices of nearest neighbors to the node
            map[i].neighbor = sorted;

            return;
        }
    }
}
