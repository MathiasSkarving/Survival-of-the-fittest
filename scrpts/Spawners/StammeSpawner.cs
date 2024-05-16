using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StammeSpawner : MonoBehaviour
{

    public GameObject campFire;
    public void SpawnStamme(float x, float z)
    {
        Vector3 pos = new Vector3(x, GameManager.getHeight(x,z),z);
        Instantiate(campFire,pos,Quaternion.identity,transform);
    }
}
