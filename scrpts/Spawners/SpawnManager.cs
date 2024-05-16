using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public CowSpawner cowSpawner;

    public StammeSpawner stammeSpawner;
    public int startCows;
    public int startStammer;
    public float minDistBetweenStammer;

    private List<Vector2> prevStammePos = new List<Vector2>();

    private bool initialized = false;
    public float cowSpawnDelayRate;
    private float counter;

    public void Initialize()
    {

        cowSpawner.Initialize();
        SpawnCows(startCows);
        SpawnStamme(startStammer, minDistBetweenStammer);
        initialized = true;
    }


    void Update()
    {
        if (initialized)
        {
            counter += Time.deltaTime;
            if (counter > cowSpawnDelayRate)
            {
                counter -= cowSpawnDelayRate;
                SpawnCows(1);
            }
        }
    }


    public void SpawnCows(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2 pos = new Vector2(Random.Range(-1200, 1200), Random.Range(-1200, 1200));
            while (GameManager.getTerrain(pos.x, pos.y) != "Land")
            {
                pos = new Vector2(Random.Range(-1200, 1200), Random.Range(-1200, 1200));
            }
            cowSpawner.spawnCow(pos.x, pos.y);
        }
    }

    public void SpawnStamme(int startStammer, float minDistBetweenStammer)
    {
        for (int i = 0; i < startStammer; i++)
        {
            Vector2 xzPos = new Vector2(Random.Range(-1200, 1200), Random.Range(-1200, 1200));
            for (int j = 0; j < prevStammePos.Count; j++)
            {
                while(Vector2.Distance(xzPos, prevStammePos[j]) < minDistBetweenStammer)
                {
                    xzPos = new Vector2(Random.Range(-1200, 1200), Random.Range(-1200, 1200));
                }
            }
            while (GameManager.getTerrain(xzPos.x, xzPos.y) != "Land")
            {
                xzPos = new Vector2(Random.Range(-1200, 1200), Random.Range(-1200, 1200));
                for (int j = 0; j < prevStammePos.Count; j++)
                {
                    while(Vector2.Distance(xzPos, prevStammePos[j]) < minDistBetweenStammer)
                    {
                        xzPos = new Vector2(Random.Range(-1200, 1200), Random.Range(-1200, 1200));
                    }
                }
            }
            stammeSpawner.SpawnStamme(xzPos.x, xzPos.y);
            prevStammePos.Add(xzPos);
        }
    }
}
