using UnityEngine;
using System.Collections;

public class TerrainColor : MonoBehaviour {

    public Terrain t;
    void Start()
    {
        paintCircle(250, 250, 30, 2);
    }

    void paintCircle(int x, int y, int radius, int layer)
    {
        float[,,] map = t.terrainData.GetAlphamaps(x - radius, y - radius, radius * 2, radius * 2);

        for (int _z = 0; _z < radius * 2; _z++)
        {
            for( int _x = 0; _x < radius*2; _x++)
            {
                map[_x, _z, layer] = 1;
            }
        }

        t.terrainData.SetAlphamaps(x - radius, y - radius, map);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
