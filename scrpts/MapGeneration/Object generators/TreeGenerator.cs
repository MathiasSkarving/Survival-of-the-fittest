using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public GameObject tree;
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
    public void GenerateTrees(int seed)
    {
        int mapSize = (MapGenerator.mapChunkSize - 1) * 10;
        noiseMap = Noise.GenerateNoiseMap(mapSize, mapSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {

                if (GameManager.getTerrain(x - mapSize / 2, y - mapSize / 2) == "Land")
                {
                    noiseMap[x, y] = noiseHeightCurve.Evaluate(noiseMap[x, y]);
                    Vector2 xzPos = new Vector2(x - mapSize / 2, y - mapSize / 2);
                    Vector3 pos = new Vector3(xzPos.x, GameManager.getHeight(xzPos.x, xzPos.y), xzPos.y);
                    if (noiseMap[x, y] * chanceMultiplier > Random.Range(0, 1000000))
                    {
                        GameObject treeObject = Instantiate(tree, pos, transform.rotation, transform);
                        RaycastHit hit;
                        if (Physics.Raycast(treeObject.transform.position + Vector3.up * 20, Vector2.down, out hit))
                        {
                            Vector3 normal = hit.normal;
                            treeObject.transform.up = normal;
                            treeObject.transform.RotateAround(treeObject.transform.position, treeObject.transform.up, Random.Range(0, 360));
                        }
                    }
                }
            }
        }
    }
    public void RemoveTrees()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
