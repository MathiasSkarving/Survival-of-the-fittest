using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowSpawner : MonoBehaviour
{

    public GameObject cow;
    

    public void Initialize()
    {
    }

    public void spawnCow(float x, float z){
        Vector3 pos = new Vector3(x, GameManager.getHeight(x,z),z);
        Instantiate(cow,pos,Quaternion.identity,transform);
    }


}
