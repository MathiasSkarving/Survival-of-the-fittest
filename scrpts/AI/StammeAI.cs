using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Animations;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class StammeAI : MonoBehaviour
{
    private GameObject parent;
    public GameObject man;
    public GameObject house;
    Vector3 housepos;
    public int startAmount;
    public float maxSpawnDistFromCamp;
    public int food = 0;
    public int wood = 0;
    public int stone = 0;
    Vector3 factionColor;
    private List<ManAi> men;

    void Start()
    {
        men = new List<ManAi>();
        factionColor = new Vector3(UnityEngine.Random.Range(0, 255) / 255f, UnityEngine.Random.Range(0, 255) / 255f, UnityEngine.Random.Range(0, 255) / 255f);
        for (int i = 0; i < startAmount; i++)
        {
            spawnMan();
        }
    }

    void Update()
    {
        if (men.Count <= 0)
        {
            ManAi[] otherMenAi = FindObjectsOfType<ManAi>();
            for (int i = 0; i < otherMenAi.Length; i++)
            {
                otherMenAi[i].warMode = false;
            }
            Destroy(gameObject);
        }

        if (food >= 3)
        {
            food -= 3;
            spawnMan();
        }

        if (wood >= 5)
        {   
            Vector2 randompos = new Vector2(GameManager.random4Interval(20,100,-20,-100),GameManager.random4Interval(20,100,-20,-100));
            Vector2 parentpos = new Vector2(transform.position.x, transform.position.z);
            housepos = new Vector3(randompos.x+parentpos.x, GameManager.getHeight(randompos.x+parentpos.x, randompos.y+parentpos.y), randompos.y+parentpos.y);

            GameObject houseObject = GameObject.Instantiate(house, housepos, Quaternion.identity, transform);
            RaycastHit hit;
            if (Physics.Raycast(houseObject.transform.position + Vector3.up * 20, Vector2.down, out hit))
            {
                Vector3 normal = hit.normal;
                houseObject.transform.up = normal;
                houseObject.transform.RotateAround(houseObject.transform.position, houseObject.transform.up, Random.Range(0, 360));
            }

            for (int i = 0; i < men.Count; i++)
            {
                men[i].AddHealth(50);
            }
            wood -= 5;
        }

        if (stone >= 6)
        {
            // add damage when collecting enough stone
            for (int i = 0; i < men.Count; i++)
            {
                men[i].AddDamage(5);
            }
            stone -= 6;
        }
    }

    public void AddResources(int food, int wood, int stone)
    {
        this.food += food;
        this.wood += wood;
        this.stone += stone;
    }

    public int GetFood()
    {
        return food;
    }

    private void spawnMan()
    {
        Vector2 xzOffset = new Vector2(
            Random.Range(-maxSpawnDistFromCamp, maxSpawnDistFromCamp),
            Random.Range(-maxSpawnDistFromCamp, maxSpawnDistFromCamp)
        );
        while (
            !GameManager.validSpawn(
                xzOffset.x + transform.position.x,
                xzOffset.y + transform.position.z
            )
            || GameManager.getTerrain(
                xzOffset.x + transform.position.x,
                xzOffset.y + transform.position.z
            ) != "Land"
        )
        {
            xzOffset = new Vector2(
                Random.Range(-maxSpawnDistFromCamp, maxSpawnDistFromCamp),
                Random.Range(-maxSpawnDistFromCamp, maxSpawnDistFromCamp)
            );
        }
        Vector3 pos = new Vector3(
            xzOffset.x + transform.position.x,
            GameManager.getHeight(
                xzOffset.x + transform.position.x,
                xzOffset.y + transform.position.z
            ),
            xzOffset.y + transform.position.z
        );
        GameObject manObject = Instantiate(man, pos, Quaternion.identity, transform);
        GameObject faction = manObject.transform.GetChild(3).gameObject;
        MeshRenderer renderer = faction.GetComponent<MeshRenderer>();
        renderer.material.SetColor("_Color", new Color(factionColor.x, factionColor.y, factionColor.z));
        renderer.material.SetColor("_EmissionColor", new Color(factionColor.x, factionColor.y, factionColor.z));
        men.Add(manObject.GetComponent<ManAi>());
    }

    public void RemoveMan(ManAi man)
    {
        for (int i = 0; i < men.Count; i++)
        {
            if (men[i] == man)
            {
                men.RemoveAt(i);
            }
        }
    }
}
