using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneGenerator : MonoBehaviour
{
    public GameObject stone;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;
    public int chanceMultiplier;
    public AnimationCurve noiseHeightCurve;
    private float[,] noiseMap;
    public void Generatestones(int seed)
    {
        string[] validTerrains = { "Mountain start start", "Mountain start", "Mountain" };
        int mapSize = (MapGenerator.mapChunkSize - 1) * 10;
        noiseMap = Noise.GenerateNoiseMap(mapSize, mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                for (int i = 0; i < validTerrains.Length; i++)
                {

                    noiseMap[x, y] = noiseHeightCurve.Evaluate(noiseMap[x, y]);
                    Vector2 xzPos = new Vector2(x - mapSize / 2, y - mapSize / 2);
                    Vector3 pos = new Vector3(xzPos.x, GameManager.getHeight(xzPos.x, xzPos.y), xzPos.y);
                    if (noiseMap[x, y] * chanceMultiplier > Random.Range(0, 1000000))
                    {
                        if (GameManager.getTerrain(x - mapSize / 2, y - mapSize / 2) == validTerrains[i])
                        {
                            GameObject stoneObject = Instantiate(stone, pos, transform.rotation, transform);
                            RaycastHit hit;
                            if (Physics.Raycast(stoneObject.transform.position + Vector3.up * 20, Vector2.down, out hit))
                            {
                                Vector3 normal = hit.normal;
                                stoneObject.transform.up = normal;
                                stoneObject.transform.RotateAround(stoneObject.transform.position, stoneObject.transform.up, Random.Range(0, 360));
                            }
                        }
                    }
                }
            }
        }
    }
    public void Removestones()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
